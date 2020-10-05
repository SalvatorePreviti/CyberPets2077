using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CyberPets.Domain.PetKinds;
using CyberPets.Domain.UserPets;

namespace CyberPets.Infrastructure
{
    public class UserPetsInMemoryRepository : IUserPetsRepository
    {
        private readonly ConcurrentDictionary<Guid, InMemoryPet> _pets = new ConcurrentDictionary<Guid, InMemoryPet>();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, InMemoryPet>> _userPets = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, InMemoryPet>>();

        public Task Insert(UserPetEntity pet)
        {
            var newEntry = new InMemoryPet(pet);

            if (_pets.GetOrAdd(newEntry.Id, newEntry) != newEntry)
                throw new InvalidOperationException($"Pet {pet.Id} already exists");

            _userPets.GetOrAdd(pet.UserId, _ => new ConcurrentDictionary<Guid, InMemoryPet>()).GetOrAdd(pet.Id, newEntry);

            return Task.CompletedTask;
        }

        public Task<UserPetEntity?> GetById(Guid id) =>
            Task.FromResult(_pets.GetValueOrDefault(id)?.ToEntity());

        public Task<IEnumerable<UserPetEntity>> ListByUserId(string userId)
        {
            var result = _userPets.GetValueOrDefault(userId)?.Values.Select(entry => entry.ToEntity());
            return Task.FromResult(result?.ToArray() ?? Enumerable.Empty<UserPetEntity>());
        }

        public Task<bool> DeleteById(Guid id)
        {
            _pets.TryRemove(id, out var entry);
            if (entry == null)
                return Task.FromResult(false);

            _userPets.GetValueOrDefault(entry.UserId)?.TryRemove(entry.Id, out _);
            return Task.FromResult(true);
        }

        public Task<bool> UpdateHappiness(Guid id, UserPetMetricValue oldValue, UserPetMetricValue newValue)
        {
            var entry = _pets.GetValueOrDefault(id);
            return Task.FromResult(entry != null && UpdateInMemoryMetric(ref entry.HappinessMetric, oldValue, newValue));
        }

        public Task<bool> UpdateHunger(Guid id, UserPetMetricValue oldValue, UserPetMetricValue newValue)
        {
            var entry = _pets.GetValueOrDefault(id);
            return Task.FromResult(entry != null && UpdateInMemoryMetric(ref entry.HungerMetric, oldValue, newValue));
        }

        private static bool UpdateInMemoryMetric(ref InMemoryPetMetric field, UserPetMetricValue oldValue, UserPetMetricValue newValue)
        {
            var old = Volatile.Read(ref field);

            if (old.Metric.LastUpdate != oldValue.LastUpdate || old.Metric.LastValue != oldValue.LastValue)
                return false;

            return old == Interlocked.CompareExchange(ref field, new InMemoryPetMetric(newValue), old);
        }

        private class InMemoryPet
        {
            public readonly Guid Id;

            public readonly string UserId;

            public readonly DateTime CreationDate;

            public readonly PetKind Kind;

            public InMemoryPetMetric HungerMetric;

            public InMemoryPetMetric HappinessMetric;

            public InMemoryPet(UserPetEntity pet)
            {
                Id = pet.Id;
                UserId = pet.UserId;
                CreationDate = pet.CreationDate;
                Kind = pet.Kind;
                HungerMetric = new InMemoryPetMetric(pet.HungerMetric);
                HappinessMetric = new InMemoryPetMetric(pet.HappinessMetric);
            }

            public UserPetEntity ToEntity() =>
                new UserPetEntity(
                    userId: UserId,
                    id: Id,
                    creationDate: CreationDate,
                    kind: Kind,
                    hungerMetric: HungerMetric.Metric,
                    happinessMetric: HappinessMetric.Metric
                );
        }

        private class InMemoryPetMetric
        {
            public readonly UserPetMetricValue Metric;

            public InMemoryPetMetric(UserPetMetricValue level)
            {
                Metric = level;
            }
        }
    }
}