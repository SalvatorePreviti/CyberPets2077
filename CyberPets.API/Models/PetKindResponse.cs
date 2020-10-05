using System;
using System.ComponentModel.DataAnnotations;

namespace CyberPets.API.Models.UserPets
{
    public class PetKindResponse
    {
        public string Name { get; set; } = "";

        public double HungerRateInSeconds { get; set; }

        public double HappinessRateInSeconds { get; set; }
    }
}
