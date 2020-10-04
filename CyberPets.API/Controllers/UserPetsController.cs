using System;
using System.Linq;
using System.Threading.Tasks;
using CyberPets.Domain;
using CyberPets.API.Models;
using CyberPets.API.Models.UserPets;
using CyberPets.API.Services;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ListResponse<UserPetResponse>> List([FromRoute] UserRequest request)
        {
            var pets = await _userPetsService.List(request.UserId);
            var now = _timeProvider.Now();
            return new ListResponse<UserPetResponse> { List = pets.Select(pet => pet.ToUserPetResponse(now)) };
        }

        [HttpGet, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserPetResponse>> GetOne([FromRoute] UserPetsRouteRequest request)
        {
            var pet = await _userPetsService.GetOne(request.UserId, request.Id);
            if (pet == null)
            {
                return NotFound("Pet not found");
            }
            return Ok(pet.ToUserPetResponse(_timeProvider.Now()));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserPetResponse>> Create([FromRoute] UserPetsCreateRequest request)
        {
            var petKind = _petKinds.GetByName(request.Kind);
            if (petKind == null)
            {
                return BadRequest("Unknown pet kind");
            }
            var pet = await _userPetsService.Create(request.UserId, Guid.NewGuid(), petKind);
            return Ok(pet);
        }

        [HttpDelete, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete([FromRoute] UserPetsRouteRequest request)
        {
            if (!await _userPetsService.DeleteOne(request.UserId, request.Id))
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPut, Route("{id}/caress")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Caress([FromRoute] UserPetsRouteRequest request)
        {
            var pet = await _userPetsService.GetOne(request.UserId, request.Id);
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Feed([FromRoute] UserPetsRouteRequest request)
        {
            var pet = await _userPetsService.GetOne(request.UserId, request.Id);
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
