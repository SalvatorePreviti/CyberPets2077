using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CyberPets.API.Models
{
    public class UserRequest
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_-]{1,16}$")]
        [FromHeader(Name = "X-USER-ID")]
        public string UserId { get; set; }
    }
}
