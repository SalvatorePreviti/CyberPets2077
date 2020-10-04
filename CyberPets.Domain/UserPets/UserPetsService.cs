using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CyberPets.Infrastructure;
using CyberPets.Domain;

namespace CyberPets.API.Services
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

        public Task<IEnumerable<UserPetEntity>> List(string userId) =>
            _repository.List(userId);

        public Task<UserPetEntity> GetOne(string userId, Guid id) =>
            _repository.GetOne(userId, id);

        public async Task<UserPetEntity> Create(string userId, Guid id, PetKind kind)
        {
            var pet = new UserPetEntity(
                userId: userId,
                id: id,
                creationDate: _timeProvider.Now(),
                kind: kind
            );

            await _repository.InsertOne(pet);
            return pet;
        }

        public Task<bool> DeleteOne(string userId, Guid id) =>
            _repository.DeleteOne(userId, id);

        public async Task<bool> Caress(UserPetEntity pet) =>
            pet != null && await _repository.UpdateHappinessMetric(pet.UserId, pet.Id, pet.HappinessMetric, pet.UpdatedHappiness(_timeProvider.Now()));

        public async Task<bool> Feed(UserPetEntity pet) =>
            pet != null && await _repository.UpdateHungerMetric(pet.UserId, pet.Id, pet.HungerMetric, pet.UpdatedHunger(_timeProvider.Now()));
    }
}