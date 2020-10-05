using System;
using System.ComponentModel.DataAnnotations;

namespace CyberPets.API.Models.UserPets
{
    public class UserPetResponse
    {
        public Guid Id { get; set; }

        public string Kind { get; set; } = "";

        public DateTime CreationDate { get; set; }

        public double Hunger { get; set; }

        public double Happiness { get; set; }
    }
}
