using System;
using CyberPets.Domain;
using CyberPets.API.Models.UserPets;

namespace CyberPets.API
{
    public static class UserPetMapper
    {
        public static UserPetResponse ToUserPetResponse(this UserPetEntity entity, DateTime now) =>
             new UserPetResponse
             {
                 Id = entity.Id,
                 Kind = entity.Kind.Name,
                 CreationDate = entity.CreationDate,
                 Hunger = entity.GetHunger(now),
                 Happiness = entity.GetHappiness(now),
             };
    }
}