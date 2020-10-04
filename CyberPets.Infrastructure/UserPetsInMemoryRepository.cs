using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CyberPets.Domain;

namespace CyberPets.Infrastructure
{
    public class UserPetsInMemoryRepository : IUserPetsRepository
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<Guid, InMemoryPet>> _userPets = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, InMemoryPet>>();

        public Task<UserPetEntity> GetOne(string userId, Guid id)
        {
            return Task.FromResult(GetInMemoryPet(userId, id)?.ToEntity(userId, id));
        }

        public Task<IEnumerable<UserPetEntity>> List(string userId)
        {
            var result = _userPets.GetValueOrDefault(userId)?.Select(keyValuePair => keyValuePair.Value.ToEntity(userId, keyValuePair.Key));
            return Task.FromResult(result?.ToArray() ?? Enumerable.Empty<UserPetEntity>());
        }

        public Task<bool> DeleteOne(string userId, Guid id)
        {
            return Task.FromResult(_userPets.GetValueOrDefault(userId)?.TryRemove(id, out var _) ?? false);
        }

        public Task InsertOne(UserPetEntity pet)
        {
            var entry = new InMemoryPet(
                pet.CreationDate,
                pet.Kind,
                pet.HungerMetric,
                pet.HappinessMetric
            );

            var inMemoryPet = _userPets.GetOrAdd(pet.UserId, _ => new ConcurrentDictionary<Guid, InMemoryPet>())
                .GetOrAdd(pet.Id, _ => entry);

            if (inMemoryPet != entry)
            {
                throw new InvalidOperationException($"Pet {pet.Id} for user {pet.UserId} already exists");
            }

            return Task.CompletedTask;
        }

        public Task<bool> UpdateHappinessMetric(string userId, Guid id, UserPetMetricValue oldValue, UserPetMetricValue newValue)
        {
            var inMemoryPet = GetInMemoryPet(userId, id);
            return Task.FromResult(inMemoryPet != null && UpdateInMemoryMetric(ref inMemoryPet.HappinessMetric, oldValue, newValue));
        }

        public Task<bool> UpdateHungerMetric(string userId, Guid id, UserPetMetricValue oldValue, UserPetMetricValue newValue)
        {
            var inMemoryPet = GetInMemoryPet(userId, id);
            return Task.FromResult(inMemoryPet != null && UpdateInMemoryMetric(ref inMemoryPet.HungerMetric, oldValue, newValue));
        }

        private InMemoryPet GetInMemoryPet(string userId, Guid id) =>
             _userPets.GetValueOrDefault(userId)?.GetValueOrDefault(id);

        private static bool UpdateInMemoryMetric(ref InMemoryPetMetric field, UserPetMetricValue oldValue, UserPetMetricValue newValue)
        {
            var oldInstance = Volatile.Read(ref field);
            return
                oldInstance.Metric.LastUpdate == newValue.LastUpdate &&
                oldInstance.Metric.LastValue == newValue.LastValue &&
                oldInstance == Interlocked.CompareExchange(ref field, new InMemoryPetMetric(newValue), field);
        }

        private class InMemoryPet
        {
            public readonly DateTime CreationDate;

            public readonly PetKind Kind;

            public InMemoryPetMetric HungerMetric;

            public InMemoryPetMetric HappinessMetric;

            public InMemoryPet(DateTime creationDate, PetKind kind, UserPetMetricValue hungerMetric, UserPetMetricValue happinessMetric)
            {
                CreationDate = creationDate;
                Kind = kind;
                HungerMetric = new InMemoryPetMetric(hungerMetric);
                HappinessMetric = new InMemoryPetMetric(happinessMetric);
            }

            public UserPetEntity ToEntity(string userId, Guid id) =>
                new UserPetEntity(
                    userId: userId,
                    id: id,
                    creationDate: CreationDate,
                    kind: Kind,
                    hungerMetric: HungerMetric.Metric,
                    happinessMetric: HappinessMetric.Metric
                );
        }

        private class InMemoryPetMetric
        {
            public readonly UserPetMetricValue Metric;

            public InMemoryPetMetric(UserPetMetricValue metric)
            {
                Metric = metric;
            }
        }
    }
}