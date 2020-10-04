using System.ComponentModel.DataAnnotations;

namespace CyberPets.API.Models.UserPets
{
    public class UserPetsCreateRequest : UserRequest
    {
        [Required]
        public string Kind { get; set; }
    }
}
