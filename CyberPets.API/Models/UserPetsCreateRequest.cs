using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CyberPets.API.Models.UserPets
{
    public class UserPetsCreateRequest
    {
        [Required]
        public string? Kind { get; set; }
    }
}
