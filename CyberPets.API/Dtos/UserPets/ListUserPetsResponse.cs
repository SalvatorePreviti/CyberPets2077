using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CyberPets.API.Dtos.UserPets
{
    public class ListUserPetResponse
    {
        [Required]
        public IEnumerable<GetUserPetResponse> Items { get; set; }
    }
}
