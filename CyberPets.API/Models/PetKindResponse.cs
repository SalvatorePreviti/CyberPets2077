using System.ComponentModel.DataAnnotations;

namespace CyberPets.API.Models
{
    public class PetKindResponse
    {
        [Required]
        public string? Name { get; set; }

        public int HungerRateInSeconds { get; set; }

        public int HappinessRateInSeconds { get; set; }
    }
}
