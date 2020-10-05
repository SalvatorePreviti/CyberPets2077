using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CyberPets.Domain.UserPets
{
    public interface IUserPetsRepository
    {
        Task Insert(UserPetEntity pet);

        Task<UserPetEntity?> GetById(Guid id);

        Task<IEnumerable<UserPetEntity>> ListByUserId(string userId);

        Task<bool> DeleteById(Guid id);

        Task<bool> UpdateHappiness(Guid id, UserPetMetricValue oldValue, UserPetMetricValue newValue);

        Task<bool> UpdateHunger(Guid id, UserPetMetricValue oldValue, UserPetMetricValue newValue);
    }
}