using System;
using System.Threading.Tasks;
using CyberPets.Domain.PetKinds;
using CyberPets.Domain.UserPets;
using Xunit;

namespace CyberPets.Test.Domain.UserPets
{
    public class UserPetEntity_Test
    {
        [Fact]
        public void Constructor_initializes_all_fields()
        {
            var userId = "user123";
            var id = Guid.NewGuid();
            var creationDate = DateTime.Now;

            var hungerMetric = new UserPetMetricValue(creationDate.AddDays(1), 1.5);
            var happinessMetric = new UserPetMetricValue(creationDate.AddHours(1), 2.5);

            var entity = new UserPetEntity(
                userId: userId,
                id: id,
                creationDate: creationDate,
                kind: PetKind.Dog,
                hungerMetric: hungerMetric,
                happinessMetric: happinessMetric
            );

            Assert.Equal(userId, entity.UserId);
            Assert.Equal(id, entity.Id);
            Assert.Equal(creationDate, entity.CreationDate);
            Assert.Equal(PetKind.Dog, entity.Kind);
            Assert.Equal(hungerMetric.LastUpdate, entity.HungerMetric.LastUpdate);
            Assert.Equal(hungerMetric.LastValue, entity.HungerMetric.LastValue);
            Assert.Equal(happinessMetric.LastUpdate, entity.HappinessMetric.LastUpdate);
            Assert.Equal(happinessMetric.LastValue, entity.HappinessMetric.LastValue);
        }

        [Fact]
        public void Constructor_initializes_default_metrics()
        {
            var userId = "user123";
            var id = Guid.NewGuid();
            var creationDate = DateTime.Now;

            var entity = new UserPetEntity(
                userId: userId,
                id: id,
                creationDate: creationDate,
                kind: PetKind.Dog
            );

            Assert.Equal(userId, entity.UserId);
            Assert.Equal(id, entity.Id);
            Assert.Equal(creationDate, entity.CreationDate);
            Assert.Equal(PetKind.Dog, entity.Kind);
            Assert.Equal(creationDate, entity.HungerMetric.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue, entity.HungerMetric.LastValue);
            Assert.Equal(creationDate, entity.HappinessMetric.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue, entity.HappinessMetric.LastValue);
        }

        [Fact]
        public void GetHunger_increases_over_time()
        {
            var kind = new PetKind(name: "custom", hungerRateInSeconds: 2.5, happinessRateInSeconds: 2.5);
            var startTime = DateTime.Now;
            var entity = new UserPetEntity(
                userId: "user456",
                id: Guid.NewGuid(),
                creationDate: startTime,
                kind: kind
            );

            Assert.Equal(UserPetMetricValue.NeutralValue, entity.GetHunger(startTime));

            Assert.Equal(UserPetMetricValue.NeutralValue, entity.GetHunger(startTime.AddSeconds(2.4)));
            Assert.Equal(UserPetMetricValue.NeutralValue + 1, entity.GetHunger(startTime.AddSeconds(2.5)));
            Assert.Equal(UserPetMetricValue.NeutralValue + 1, entity.GetHunger(startTime.AddSeconds(2.6)));

            Assert.Equal(UserPetMetricValue.NeutralValue + 2, entity.GetHunger(startTime.AddSeconds(5)));

            Assert.Equal(UserPetMetricValue.MaxValue, entity.GetHunger(startTime.AddYears(1)));
        }

        [Fact]
        public void UpdatedHunger_decreases_hunger()
        {
            var kind = new PetKind(name: "custom", hungerRateInSeconds: 2.5, happinessRateInSeconds: 2.5);
            var startTime = DateTime.Now;
            var entity = new UserPetEntity(
                userId: "user456",
                id: Guid.NewGuid(),
                creationDate: startTime,
                kind: kind
            );

            var updated = entity.UpdatedHunger(startTime, 0);
            Assert.Equal(startTime, updated.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue, updated.LastValue);

            updated = entity.UpdatedHunger(startTime, 1);
            Assert.Equal(startTime, updated.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue - 1, updated.LastValue);

            updated = entity.UpdatedHunger(startTime.AddSeconds(2.5), 1);
            Assert.Equal(startTime.AddSeconds(2.5), updated.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue, updated.LastValue);

            updated = entity.UpdatedHunger(startTime.AddSeconds(5), 1);
            Assert.Equal(startTime.AddSeconds(5), updated.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue + 1, updated.LastValue);

            updated = entity.UpdatedHunger(startTime, 10000);
            Assert.Equal(startTime, updated.LastUpdate);
            Assert.Equal(UserPetMetricValue.MinValue, updated.LastValue);
        }

        [Fact]
        public void GetHappiness_decreases_over_time()
        {
            var kind = new PetKind(name: "custom", hungerRateInSeconds: 2.5, happinessRateInSeconds: 2.5);
            var startTime = DateTime.Now;
            var entity = new UserPetEntity(
                userId: "user456",
                id: Guid.NewGuid(),
                creationDate: startTime,
                kind: kind
            );

            Assert.Equal(UserPetMetricValue.NeutralValue, entity.GetHappiness(startTime));

            Assert.Equal(UserPetMetricValue.NeutralValue, entity.GetHappiness(startTime.AddSeconds(2.4)));
            Assert.Equal(UserPetMetricValue.NeutralValue - 1, entity.GetHappiness(startTime.AddSeconds(2.5)));
            Assert.Equal(UserPetMetricValue.NeutralValue - 1, entity.GetHappiness(startTime.AddSeconds(2.6)));

            Assert.Equal(UserPetMetricValue.NeutralValue - 2, entity.GetHappiness(startTime.AddSeconds(5)));

            Assert.Equal(UserPetMetricValue.MinValue, entity.GetHappiness(startTime.AddYears(1)));
        }

        [Fact]
        public void UpdatedHappiness_increases_happiness()
        {
            var kind = new PetKind(name: "custom", hungerRateInSeconds: 2.5, happinessRateInSeconds: 2.5);
            var startTime = DateTime.Now;
            var entity = new UserPetEntity(
                userId: "user456",
                id: Guid.NewGuid(),
                creationDate: startTime,
                kind: kind
            );

            var updated = entity.UpdatedHappiness(startTime, 0);
            Assert.Equal(startTime, updated.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue, updated.LastValue);

            updated = entity.UpdatedHappiness(startTime, 1);
            Assert.Equal(startTime, updated.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue + 1, updated.LastValue);

            updated = entity.UpdatedHappiness(startTime.AddSeconds(2.5), 1);
            Assert.Equal(startTime.AddSeconds(2.5), updated.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue, updated.LastValue);

            updated = entity.UpdatedHappiness(startTime.AddSeconds(5), 1);
            Assert.Equal(startTime.AddSeconds(5), updated.LastUpdate);
            Assert.Equal(UserPetMetricValue.NeutralValue - 1, updated.LastValue);

            updated = entity.UpdatedHappiness(startTime, 10000);
            Assert.Equal(startTime, updated.LastUpdate);
            Assert.Equal(UserPetMetricValue.MaxValue, updated.LastValue);
        }
    }
}