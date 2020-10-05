using System;
using System.Threading.Tasks;
using CyberPets.API.Mappers;
using CyberPets.Domain.PetKinds;
using CyberPets.Domain.UserPets;
using Xunit;

namespace CyberPets.Test.Models
{
    public class UserPetMapper_Test
    {
        [Fact]
        public void It_maps_correctly()
        {
            var kind = new PetKind(name: "hello", hungerRateInSeconds: 123, happinessRateInSeconds: 456);

            var userId = "user123";
            var id = Guid.NewGuid();
            var creationDate = new DateTime(2030, 4, 5);

            var queryDate = creationDate.AddDays(4.5);

            var hungerMetric = new UserPetMetricValue(creationDate.AddDays(1), 1);
            var happinessMetric = new UserPetMetricValue(creationDate.AddHours(1), 2);

            var pet = new UserPetEntity(
                userId: userId,
                id: id,
                creationDate: creationDate,
                kind: PetKind.Dog,
                hungerMetric: hungerMetric,
                happinessMetric: happinessMetric
            );

            var mapped = pet.ToUserPetResponse(queryDate);

            Assert.Equal(id, mapped.Id);
            Assert.Equal(creationDate, mapped.CreationDate);
            Assert.Equal(pet.GetHunger(queryDate), mapped.Hunger);
            Assert.Equal(pet.GetHappiness(queryDate), mapped.Happiness);
        }
    }
}