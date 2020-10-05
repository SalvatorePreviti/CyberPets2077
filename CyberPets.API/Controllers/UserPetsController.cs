using System;
using System.Linq;
using System.Threading.Tasks;
using CyberPets.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using CyberPets.Domain.UserPets;
using CyberPets.Domain.PetKinds;
using CyberPets.API.Mappers;
using CyberPets.Domain.Time;

namespace CyberPets.API.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class UserPetsController : ControllerBase
    {
        public const string UserIdHeader = "X-USER-ID";

        private readonly ITimeProvider _timeProvider;
        private readonly PetKindsList _petKinds;
        private readonly UserPetsService _userPetsService;

        public UserPetsController(ITimeProvider timeProvider, PetKindsList petKinds, UserPetsService userPetsService)
        {
            _timeProvider = timeProvider;
            _petKinds = petKinds;
            _userPetsService = userPetsService;
        }

        [HttpGet]
        [Description("Lists all the pets owned by a user.")]
        public async Task<IEnumerable<UserPetResponse>> List(
            [FromHeader(Name = UserIdHeader), Required] string userId
        )
        {
            var pets = await _userPetsService.ListByUserId(userId);
            var now = _timeProvider.Now();
            return pets.Select(pet => pet.ToUserPetResponse(now));
        }

        [HttpGet, Route("{id}")]
        public async Task<ActionResult<UserPetResponse>> GetOne(
            [FromHeader(Name = UserIdHeader), Required] string userId,
            [FromRoute] Guid id
        )
        {
            var pet = await _userPetsService.GetById(id);
            if (pet == null)
                return NotFound("Pet not found");

            if (pet.UserId != userId)
                return Unauthorized();

            return Ok(pet.ToUserPetResponse(_timeProvider.Now()));
        }

        [HttpPost]
        public async Task<ActionResult<UserPetResponse>> Create(
            [FromHeader(Name = UserIdHeader), Required] string userId,
            [FromBody] UserPetsCreateRequest request
        )
        {
            var kind = _petKinds.GetByName(request.Kind);
            if (kind == null)
                return BadRequest("Unknown pet kind");

            var pet = await _userPetsService.Create(userId, kind);
            return Ok(pet.ToUserPetResponse(_timeProvider.Now()));
        }

        [HttpDelete, Route("{id}")]
        public async Task<ActionResult> Delete(
            [FromHeader(Name = UserIdHeader), Required] string userId,
            [FromRoute] Guid id
        )
        {
            var pet = await _userPetsService.GetById(id);

            if (pet == null)
                return NotFound();

            if (pet.UserId != userId)
                return Unauthorized();

            if (!await _userPetsService.DeleteById(id))
                return NotFound();

            return Ok();
        }

        [HttpPut, Route("{id}/caress")]
        public async Task<ActionResult<int>> Caress(
            [FromHeader(Name = UserIdHeader), Required] string userId,
            [FromRoute] Guid id
        )
        {
            var pet = await _userPetsService.GetById(id);
            if (pet == null)
                return NotFound();

            if (pet.UserId != userId)
                return Unauthorized();

            return Ok(await _userPetsService.Caress(pet));
        }

        [HttpPut, Route("{id}/feed")]
        public async Task<ActionResult<int>> Feed(
            [FromHeader(Name = UserIdHeader), Required] string userId,
            [FromRoute] Guid id
        )
        {
            var pet = await _userPetsService.GetById(id);
            if (pet == null)
                return NotFound();

            if (pet.UserId != userId)
                return Unauthorized();

            return Ok(await _userPetsService.Feed(pet));
        }
    }
}
