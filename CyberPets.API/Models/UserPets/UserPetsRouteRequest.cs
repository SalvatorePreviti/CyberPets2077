using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CyberPets.API.Models.UserPets
{
    public class UserPetsRouteRequest : UserRequest
    {
        [Required]
        [FromRoute]
        public Guid Id { get; set; }
    }
}
