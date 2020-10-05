using CyberPets.Domain;
using CyberPets.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using CyberPets.API.Models.UserPets;

namespace CyberPets.API.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class PetKindsController : ControllerBase
    {
        private readonly PetKinds _petKinds;

        public PetKindsController(UserPetsService userPetsService, PetKinds petKinds)
        {
            _petKinds = petKinds;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<PetKindResponse> List() => _petKinds.Values.Select(kind => kind.ToPetKindResponse());
    }
}
