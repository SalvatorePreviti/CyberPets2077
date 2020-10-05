using CyberPets.Domain;
using CyberPets.API.Models;
using CyberPets.Domain.PetKinds;

namespace CyberPets.API.Mappers
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