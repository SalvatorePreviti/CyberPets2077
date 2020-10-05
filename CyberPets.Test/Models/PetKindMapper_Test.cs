using System.Threading.Tasks;
using CyberPets.API.Mappers;
using CyberPets.Domain.PetKinds;
using Xunit;

namespace CyberPets.Test.Models
{
    public class PetKindMaster_Test
    {
        [Fact]
        public void It_maps_correctly()
        {
            var kind = new PetKind(name: "hello", hungerRateInSeconds: 123, happinessRateInSeconds: 456);
            var mapped = kind.ToPetKindResponse();
            Assert.Equal("hello", mapped.Name);
            Assert.Equal(123, mapped.HungerRateInSeconds);
            Assert.Equal(456, mapped.HappinessRateInSeconds);
        }
    }
}