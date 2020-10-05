using System;
using System.ComponentModel.DataAnnotations;

namespace CyberPets.API.Models
{
    public class UserPetResponse
    {
        public Guid Id { get; set; }

        [Required]
        public string? Kind { get; set; }

        public DateTime CreationDate { get; set; }

        public int Hunger { get; set; }

        public int Happiness { get; set; }
    }
}
