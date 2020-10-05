using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using CyberPets.API.Models;
using CyberPets.Domain.PetKinds;
using CyberPets.API.Mappers;

namespace CyberPets.API.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class PetKindsController : ControllerBase
    {
        private readonly PetKinds _petKinds;

        public PetKindsController(PetKinds petKinds)
        {
            _petKinds = petKinds;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<PetKindResponse> List() => _petKinds.Values.Select(kind => kind.ToPetKindResponse());
    }
}
