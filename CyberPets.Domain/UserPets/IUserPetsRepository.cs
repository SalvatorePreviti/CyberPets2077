using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CyberPets.Domain;

namespace CyberPets.Infrastructure
{
    public interface IUserPetsRepository
    {
        Task<IEnumerable<UserPetEntity>> List(string userId);

        Task<UserPetEntity> GetOne(string userId, Guid id);

        Task<bool> DeleteOne(string userId, Guid id);

        Task InsertOne(UserPetEntity pet);

        Task<bool> UpdateHappinessMetric(string userId, Guid id, UserPetMetricValue oldValue, UserPetMetricValue newValue);

        Task<bool> UpdateHungerMetric(string userId, Guid id, UserPetMetricValue oldValue, UserPetMetricValue newValue);
    }
}