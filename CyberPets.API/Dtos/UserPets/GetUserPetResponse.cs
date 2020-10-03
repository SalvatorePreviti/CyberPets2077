using System;
using System.ComponentModel.DataAnnotations;

namespace CyberPets.API.Dtos.UserPets
{
    public class GetUserPetResponse
    {
        public Guid Id { get; set; }

        [Required]
        public string Kind { get; set; }

        public DateTime CreationDate { get; set; }

        [Range(Constants.MinPetMetricValue, Constants.MaxPetMetricValue)]
        public int Hunger { get; set; }

        [Range(Constants.MinPetMetricValue, Constants.MaxPetMetricValue)]
        public int Happiness { get; set; }
    }
}
