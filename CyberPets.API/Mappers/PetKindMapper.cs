using System;
using CyberPets.Domain;
using CyberPets.API.Models.UserPets;

namespace CyberPets.API
{
    public static class PetKindMapper
    {
        public static PetKindResponse ToPetKindResponse(this PetKind kind) =>
             new PetKindResponse
             {
                 Name = kind.Name,
                 HungerRateInSeconds = kind.HungerRateInSeconds,
                 HappinessRateInSeconds = kind.HappinessRateInSeconds
             };
    }
}