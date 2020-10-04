using CyberPets.Domain;
using CyberPets.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CyberPets.API.Models;
using System.Linq;
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
        public ListResponse<PetKindResponse> List() => new ListResponse<PetKindResponse> { List = _petKinds.Values.Select(kind => kind.ToPetKindResponse()) };
    }
}
