using System;
using System.Threading.Tasks;
using CyberPets.API.Services;
using Xunit;

namespace CyberPets.Test.UserPetsAPI
{
    public class ListUserPets_GetOne_Test
    {
        private readonly UserPetsService _service;

        public ListUserPets_GetOne_Test()
        {
            _service = new UserPetsService();
        }

        [Fact]
        public async Task Returns_null_if_user_does_not_exists()
        {
            Assert.Null(await _service.GetOne("UNKNOWN", Guid.Empty));
        }

        [Fact]
        public async Task Returns_null_if_user_exists_but_pet_does_not()
        {
            Assert.Null(await _service.GetOne("user1", new Guid("0dad5e4d-5dad-4ee8-a89e-6c7083116000")));
        }

        [Fact]
        public async Task Returns_a_pet()
        {
            var pet = await _service.GetOne("user1", new Guid("0dad5e4d-5dad-4ee8-a89e-6c7083116001"));
            Assert.Null(pet);
        }
    }
}