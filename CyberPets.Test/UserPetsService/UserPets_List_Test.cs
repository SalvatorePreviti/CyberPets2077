using System.Threading.Tasks;
using CyberPets.API.Services;
using Xunit;

namespace CyberPets.Test.UserPetsAPI
{
    public class ListUserPets_Test
    {
        private readonly UserPetsService _service;

        public ListUserPets_Test()
        {
            _service = new UserPetsService();
        }

        [Fact]
        public async Task Returns_an_empty_list_if_the_user_has_no_pets()
        {
            Assert.Empty((await _service.List("UNKNOWN")).List);
        }

        [Fact]
        public async Task Returns_a_list_of_pets()
        {
            var items = (await _service.List("user1")).List;
            Assert.Empty(items);
        }
    }
}