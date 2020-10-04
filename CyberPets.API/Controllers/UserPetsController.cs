using System;
using System.Linq;
using System.Threading.Tasks;
using CyberPets.Domain;
using CyberPets.API.Models;
using CyberPets.API.Models.UserPets;
using CyberPets.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CyberPets.API.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class UserPetsController : ControllerBase
    {
        private readonly ITimeProvider _timeProvider;
        private readonly PetKinds _petKinds;
        private readonly UserPetsService _userPetsService;

        public UserPetsController(ITimeProvider timeProvider, PetKinds petKinds, UserPetsService userPetsService)
        {
            _timeProvider = timeProvider;
            _petKinds = petKinds;
            _userPetsService = userPetsService;
        }

        [HttpGet]
        public async Task<ListResponse<UserPetResponse>> List([FromForm] UserRequest userRequest)
        {
            var pets = await _userPetsService.List(userRequest.UserId);
            var now = _timeProvider.Now();
            return new ListResponse<UserPetResponse> { List = pets.Select(pet => pet.ToUserPetResponse(now)) };
        }

        [HttpGet, Route("{id}")]
        public async Task<ActionResult<UserPetResponse>> GetOne([FromForm] UserRequest userRequest, [FromRoute] Guid id)
        {
            var pet = await _userPetsService.GetOne(userRequest.UserId, id);
            if (pet == null)
            {
                return NotFound("Pet not found");
            }
            return Ok(pet.ToUserPetResponse(_timeProvider.Now()));
        }

        [HttpPost]
        public async Task<ActionResult<UserPetResponse>> Create([FromForm] UserRequest userRequest, [FromBody] UserPetsCreateRequest request)
        {
            var petKind = _petKinds.GetByName(request.Kind);
            if (petKind == null)
            {
                return BadRequest("Unknown pet kind.");
            }
            var pet = await _userPetsService.Create(userRequest.UserId, Guid.NewGuid(), petKind);
            return Ok(pet.ToUserPetResponse(_timeProvider.Now()));
        }

        [HttpDelete, Route("{id}")]
        public async Task<ActionResult> Delete([FromForm] UserRequest userRequest, [FromRoute] Guid id)
        {
            if (!await _userPetsService.DeleteOne(userRequest.UserId, id))
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPut, Route("{id}/caress")]
        public async Task<ActionResult> Caress([FromForm] UserRequest userRequest, [FromRoute] Guid id)
        {
            var pet = await _userPetsService.GetOne(userRequest.UserId, id);
            if (pet == null)
            {
                return NotFound();
            }
            if (!await _userPetsService.Caress(pet))
            {
                return new StatusCodeResult(429);
            }
            return NoContent();
        }

        [HttpPut, Route("{id}/feed")]
        public async Task<ActionResult> Feed([FromForm] UserRequest userRequest, [FromRoute] Guid id)
        {
            var pet = await _userPetsService.GetOne(userRequest.UserId, id);
            if (pet == null)
            {
                return NotFound();
            }
            if (!await _userPetsService.Feed(pet))
            {
                return new StatusCodeResult(429);
            }
            return NoContent();
        }
    }
}
