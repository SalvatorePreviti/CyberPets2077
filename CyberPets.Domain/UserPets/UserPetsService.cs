using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CyberPets.Domain.PetKinds;
using CyberPets.Domain.Time;

namespace CyberPets.Domain.UserPets
{
    public class UserPetsService
    {
        private readonly ITimeProvider _timeProvider;

        private readonly IUserPetsRepository _repository;

        public UserPetsService(ITimeProvider timeProvider, IUserPetsRepository repository)
        {
            _timeProvider = timeProvider;
            _repository = repository;
        }

        public Task<UserPetEntity?> GetById(Guid id) =>
            _repository.GetById(id);

        public Task<IEnumerable<UserPetEntity>> ListByUserId(string userId) =>
            _repository.ListByUserId(userId);

        public async Task<UserPetEntity> Create(string userId, PetKind kind)
        {
            var pet = new UserPetEntity(
                userId: userId,
                id: Guid.NewGuid(),
                creationDate: _timeProvider.Now(),
                kind: kind
            );
            await _repository.Insert(pet);
            return pet;
        }

        public Task<bool> DeleteById(Guid id) =>
            _repository.DeleteById(id);

        public async Task<bool> Caress(UserPetEntity pet) =>
            pet != null && await _repository.UpdateHappiness(pet.Id, pet.HappinessMetric, pet.UpdatedHappiness(_timeProvider.Now(), 1));

        public async Task<bool> Feed(UserPetEntity pet) =>
            pet != null && await _repository.UpdateHunger(pet.Id, pet.HungerMetric, pet.UpdatedHunger(_timeProvider.Now(), 1));
    }
}