using System;
using System.Linq;
using System.Threading.Tasks;
using CyberPets.Domain.PetKinds;
using CyberPets.Domain.UserPets;
using CyberPets.Infrastructure;
using Xunit;

namespace CyberPets.Test.Infrastructure
{
    public class UserPetsInMemoryRepository_Test
    {
        private readonly UserPetsInMemoryRepository _repository;

        public UserPetsInMemoryRepository_Test()
        {
            _repository = new UserPetsInMemoryRepository();
        }

        [Fact]
        public async Task Insert_throws_if_id_already_exists()
        {
            var id = new Guid("76aaa84f-ee34-4935-925f-f2fe68bb2e80");
            await _repository.Insert(new UserPetEntity(userId: "user123", id: id, creationDate: DateTime.Now, kind: PetKind.Cat));
            await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.Insert(new UserPetEntity(userId: "user456", id: id, creationDate: DateTime.Now, kind: PetKind.Dog)));
        }

        [Fact]
        public async Task Get_an_item_that_does_not_exists_returns_null()
        {
            Assert.Null(await _repository.GetById(Guid.NewGuid()));
        }

        [Fact]
        public async Task Inserts_an_item_and_get_by_id()
        {
            var id = Guid.NewGuid();
            DateTime creationDate = DateTime.Now;
            var inserted = new UserPetEntity(
                userId: "user123",
                id: id,
                creationDate: creationDate,
                kind: PetKind.Cat
            );
            await _repository.Insert(inserted);
            var queried = await _repository.GetById(id);
            Assert.NotNull(queried);
            Assert.Equal("user123", queried.UserId);
            Assert.Equal(id, queried.Id);
            Assert.Equal(creationDate, queried.CreationDate);
            Assert.Same(PetKind.Cat, queried.Kind);
            Assert.Equal(creationDate, queried.HungerMetric.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue, queried.HungerMetric.LastValue);
            Assert.Equal(creationDate, queried.HappinessMetric.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue, queried.HappinessMetric.LastValue);
        }

        [Fact]
        public async Task ListByUserId_returns_an_empty_array_for_not_existing_user()
        {
            Assert.Empty(await _repository.ListByUserId("not-existing-user-id"));
        }

        [Fact]
        public async Task ListByUserId_returns_items_by_user_id()
        {
            var a = new UserPetEntity(
                userId: "XUser2",
                id: Guid.NewGuid(),
                creationDate: DateTime.Now,
                kind: PetKind.Cat
            );
            var b = new UserPetEntity(
                userId: "XUser1",
                id: Guid.NewGuid(),
                creationDate: DateTime.Now,
                kind: PetKind.Dog
            );
            var c = new UserPetEntity(
                userId: "XUser2",
                id: Guid.NewGuid(),
                creationDate: DateTime.Now,
                kind: PetKind.Dragon
            );
            await _repository.Insert(a);
            await _repository.Insert(b);
            await _repository.Insert(c);

            var pets1 = (await _repository.ListByUserId("XUser1")).ToArray();
            Assert.Single(pets1, pet => pet.Id == b.Id);

            var pets2 = (await _repository.ListByUserId("XUser2")).ToArray();
            Assert.Single(pets2, pet => pet.Id == a.Id);
            Assert.Single(pets2, pet => pet.Id == c.Id);
        }

        [Fact]
        public async Task Deletes_by_id()
        {
            var pet = new UserPetEntity(
                userId: "AUserN",
                id: Guid.NewGuid(),
                creationDate: DateTime.Now,
                kind: PetKind.Cat
            );
            await _repository.Insert(pet);
            Assert.NotNull(await _repository.GetById(pet.Id));
            Assert.Single(await _repository.ListByUserId(pet.UserId));

            Assert.True(await _repository.DeleteById(pet.Id));

            Assert.Null(await _repository.GetById(pet.Id));
            Assert.Empty(await _repository.ListByUserId(pet.UserId));

            Assert.False(await _repository.DeleteById(pet.Id));
        }

        [Fact]
        public async Task Update_happyness_returns_false_if_pet_not_found()
        {
            Assert.False(await _repository.UpdateHappiness(Guid.NewGuid(), new UserPetMetricValue(), new UserPetMetricValue()));
        }

        [Fact]
        public async Task Updates_happyness_if_old_value_matches()
        {
            var pet = new UserPetEntity(
                userId: "UserH1",
                id: Guid.NewGuid(),
                creationDate: DateTime.Now,
                kind: PetKind.Cat
            );
            await _repository.Insert(pet);

            var newMetricValue = new UserPetMetricValue(pet.CreationDate.AddDays(1), UserPetMetricValue.MaxValue);

            Assert.True(await _repository.UpdateHappiness(pet.Id, pet.HappinessMetric, newMetricValue));

            var queried = (await _repository.GetById(pet.Id)).HappinessMetric;
            Assert.Equal(newMetricValue.LastValue, queried.LastValue);
            Assert.Equal(newMetricValue.LastUpdate, queried.LastUpdate);
        }

        [Fact]
        public async Task Does_not_updates_happyness_if_old_value_does_not_match()
        {
            var pet = new UserPetEntity(
                userId: "UserH2",
                id: Guid.NewGuid(),
                creationDate: DateTime.Now,
                kind: PetKind.Cat
            );
            await _repository.Insert(pet);

            var newMetricValue = new UserPetMetricValue(pet.CreationDate.AddDays(1), UserPetMetricValue.MaxValue);

            Assert.False(await _repository.UpdateHappiness(pet.Id, newMetricValue, newMetricValue));

            var queried = (await _repository.GetById(pet.Id)).HappinessMetric;
            Assert.Equal(pet.HappinessMetric.LastValue, queried.LastValue);
            Assert.Equal(pet.HappinessMetric.LastUpdate, queried.LastUpdate);
        }

        [Fact]
        public async Task Update_happiness_optimistic_lock()
        {
            var pet = new UserPetEntity(
                userId: "UserH3",
                id: Guid.NewGuid(),
                creationDate: DateTime.Now,
                kind: PetKind.Cat
            );
            await _repository.Insert(pet);

            var newMetricValue1 = new UserPetMetricValue(pet.CreationDate.AddDays(1), UserPetMetricValue.MaxValue);
            var newMetricValue2 = new UserPetMetricValue(pet.CreationDate.AddDays(1), 0);

            Assert.True(await _repository.UpdateHappiness(pet.Id, pet.HappinessMetric, newMetricValue1));
            Assert.False(await _repository.UpdateHappiness(pet.Id, pet.HappinessMetric, newMetricValue2));

            var queried = (await _repository.GetById(pet.Id)).HappinessMetric;
            Assert.Equal(newMetricValue1.LastValue, queried.LastValue);
            Assert.Equal(newMetricValue1.LastUpdate, queried.LastUpdate);
        }

        [Fact]
        public async Task Update_hunger_returns_false_if_pet_not_found()
        {
            Assert.False(await _repository.UpdateHunger(Guid.NewGuid(), new UserPetMetricValue(), new UserPetMetricValue()));
        }

        [Fact]
        public async Task Updates_hunger_if_old_value_matches()
        {
            var pet = new UserPetEntity(
                userId: "UserH1",
                id: Guid.NewGuid(),
                creationDate: DateTime.Now,
                kind: PetKind.Cat
            );
            await _repository.Insert(pet);

            var newMetricValue = new UserPetMetricValue(pet.CreationDate.AddDays(1), UserPetMetricValue.MaxValue);

            Assert.True(await _repository.UpdateHunger(pet.Id, pet.HungerMetric, newMetricValue));

            var queried = (await _repository.GetById(pet.Id)).HungerMetric;
            Assert.Equal(newMetricValue.LastValue, queried.LastValue);
            Assert.Equal(newMetricValue.LastUpdate, queried.LastUpdate);
        }

        [Fact]
        public async Task Does_not_updates_hunger_if_old_value_does_not_match()
        {
            var pet = new UserPetEntity(
                userId: "UserH2",
                id: Guid.NewGuid(),
                creationDate: DateTime.Now,
                kind: PetKind.Cat
            );
            await _repository.Insert(pet);

            var newMetricValue = new UserPetMetricValue(pet.CreationDate.AddDays(1), UserPetMetricValue.MaxValue);

            Assert.False(await _repository.UpdateHunger(pet.Id, newMetricValue, newMetricValue));

            var queried = (await _repository.GetById(pet.Id)).HungerMetric;
            Assert.Equal(pet.HungerMetric.LastValue, queried.LastValue);
            Assert.Equal(pet.HungerMetric.LastUpdate, queried.LastUpdate);
        }

        [Fact]
        public async Task Update_hunger_optimistic_lock()
        {
            var pet = new UserPetEntity(
                userId: "UserH3",
                id: Guid.NewGuid(),
                creationDate: DateTime.Now,
                kind: PetKind.Cat
            );
            await _repository.Insert(pet);

            var newMetricValue1 = new UserPetMetricValue(pet.CreationDate.AddDays(1), UserPetMetricValue.MaxValue);
            var newMetricValue2 = new UserPetMetricValue(pet.CreationDate.AddDays(1), 0);

            Assert.True(await _repository.UpdateHunger(pet.Id, pet.HungerMetric, newMetricValue1));
            Assert.False(await _repository.UpdateHunger(pet.Id, pet.HungerMetric, newMetricValue2));

            var queried = (await _repository.GetById(pet.Id)).HungerMetric;
            Assert.Equal(newMetricValue1.LastValue, queried.LastValue);
            Assert.Equal(newMetricValue1.LastUpdate, queried.LastUpdate);
        }
    }
}