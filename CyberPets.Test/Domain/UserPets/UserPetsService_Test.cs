using System;
using System.Linq;
using System.Threading.Tasks;
using CyberPets.Domain.PetKinds;
using CyberPets.Domain.UserPets;
using CyberPets.Infrastructure;
using Xunit;

namespace CyberPets.Test.Domain.UserPets
{
    public class UserPetsService_Test
    {
        [Fact]
        public async Task Get_an_item_that_does_not_exists_returns_null()
        {
            var time = new TestTimeProvider();
            var repo = new UserPetsInMemoryRepository();
            var service = new UserPetsService(time, repo);

            Assert.Null(await service.GetById(Guid.NewGuid()));
        }

        [Fact]
        public async Task ListByUserId_returns_an_empty_array_for_not_existing_user()
        {
            var time = new TestTimeProvider();
            var repo = new UserPetsInMemoryRepository();
            var service = new UserPetsService(time, repo);

            Assert.Empty(await service.ListByUserId("not-existing-user-id"));
        }

        [Fact]
        public async Task Creates_a_pet_and_get_by_id()
        {
            var time = new TestTimeProvider();
            var repo = new UserPetsInMemoryRepository();
            var service = new UserPetsService(time, repo);

            time.Value = new DateTime(2020, 1, 1);

            var pet = await service.Create(userId: "User123", PetKind.Dog);
            Assert.NotNull(pet);

            var queried = await service.GetById(pet.Id);
            Assert.NotNull(queried);

            Assert.Equal(time.Value, queried.CreationDate);
            Assert.Equal(pet.Id, queried.Id);
            Assert.Same(PetKind.Dog, queried.Kind);

            Assert.Equal(time.Value, queried.HappinessMetric.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue, queried.HappinessMetric.LastValue);

            Assert.Equal(time.Value, queried.HungerMetric.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue, queried.HungerMetric.LastValue);
        }

        [Fact]
        public async Task ListByUserId_returns_items_by_user_id()
        {
            var time = new TestTimeProvider();
            var repo = new UserPetsInMemoryRepository();
            var service = new UserPetsService(time, repo);

            time.Value = new DateTime(2020, 1, 1);

            var a = await service.Create(userId: "XUser2", PetKind.Cat);
            var b = await service.Create(userId: "XUser1", PetKind.Dog);
            var c = await service.Create(userId: "XUser2", PetKind.Dragon);

            var pets1 = (await service.ListByUserId("XUser1")).ToArray();
            Assert.Single(pets1, pet => pet.Id == b.Id);

            var pets2 = (await service.ListByUserId("XUser2")).ToArray();
            Assert.Single(pets2, pet => pet.Id == a.Id);
            Assert.Single(pets2, pet => pet.Id == c.Id);
        }

        [Fact]
        public async Task Deletes_by_id()
        {
            var time = new TestTimeProvider();
            var repo = new UserPetsInMemoryRepository();
            var service = new UserPetsService(time, repo);

            var pet = await service.Create(userId: "AUserN", PetKind.Cat);
            Assert.NotNull(await service.GetById(pet.Id));
            Assert.Single(await service.ListByUserId(pet.UserId));

            Assert.True(await service.DeleteById(pet.Id));

            Assert.Null(await service.GetById(pet.Id));
            Assert.Empty(await service.ListByUserId(pet.UserId));

            Assert.False(await service.DeleteById(pet.Id));
        }

        [Fact]
        public async Task Caress()
        {
            var kind = new PetKind("animalX", 2, 2);

            var time = new TestTimeProvider();
            var repo = new UserPetsInMemoryRepository();
            var service = new UserPetsService(time, repo);

            var pet = await service.Create(userId: "user", kind);

            time.Value = time.Value.AddSeconds(1);
            Assert.True(await service.Caress(pet));
            pet = await service.GetById(pet.Id);

            Assert.Equal(UserPetMetricValue.NeutralValue + 1, pet.GetHappiness(time.Now()));

            time.Value = time.Value.AddSeconds(0.1);
            Assert.True(await service.Caress(pet));
            pet = await service.GetById(pet.Id);

            Assert.Equal(UserPetMetricValue.NeutralValue + 1, pet.GetHappiness(time.Now()));
        }
    }
}