using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CyberPets.API.Dtos.UserPets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CyberPets.API.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class UserPetsController : ControllerBase
    {
        private readonly ILogger<UserPetsController> _logger;

        public UserPetsController(ILogger<UserPetsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Task<ListUserPetResponse> Get()
        {
            return null;
        }

        [HttpGet]
        [Route("{id}")]
        public Task<GetUserPetResponse> GetOne([Required] Guid id)
        {
            return null;
        }

        [HttpPost]
        public Task<GetUserPetResponse> Create([Required] CreateUserPetRequest request)
        {
            return null;
        }

        [HttpDelete]
        [Route("{id}")]
        public async void Delete([Required] Guid id)
        {
        }

        [HttpPut]
        [Route("{id}/caress")]
        public async void Caress([Required] Guid id)
        {
        }

        [HttpPut]
        [Route("{id}/feed")]
        public async void Feed([Required] Guid id)
        {
        }
    }
}
