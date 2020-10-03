using System.ComponentModel.DataAnnotations;

namespace CyberPets.API.Dtos.UserPets
{
    public class CreateUserPetRequest
    {
        [Required]
        public string Kind { get; set; }
    }
}
