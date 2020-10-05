using System.ComponentModel.DataAnnotations;

namespace CyberPets.API.Models
{
    public class UserPetsCreateRequest
    {
        [Required]
        public string? Kind { get; set; }
    }
}
