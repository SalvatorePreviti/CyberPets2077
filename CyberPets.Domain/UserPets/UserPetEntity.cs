using System;

namespace CyberPets.Domain
{
    public class UserPetEntity
    {
        public string UserId { get; private set; }

        public Guid Id { get; private set; }

        public DateTime CreationDate { get; private set; }

        public PetKind Kind { get; private set; }

        public UserPetMetricValue HungerMetric { get; private set; }

        public UserPetMetricValue HappinessMetric { get; private set; }

        public UserPetEntity(string userId, Guid id, DateTime creationDate, PetKind kind, UserPetMetricValue? hungerMetric = null, UserPetMetricValue? happinessMetric = null)
        {
            UserId = userId;
            Id = id;
            CreationDate = creationDate;
            Kind = kind;
            HungerMetric = hungerMetric ?? new UserPetMetricValue(CreationDate);
            HappinessMetric = happinessMetric ?? new UserPetMetricValue(CreationDate);
        }

        public int GetHunger(DateTime now) => HungerMetric.GetValue(now, Kind.HungerRateInSeconds);

        public UserPetMetricValue UpdatedHunger(DateTime now, int amount = 1) => HungerMetric.Updated(now, Kind.HungerRateInSeconds, amount);

        public int GetHappiness(DateTime now) => HappinessMetric.GetValue(now, Kind.HappinessRateInSeconds);

        public UserPetMetricValue UpdatedHappiness(DateTime now, int amount = 1) => HappinessMetric.Updated(now, Kind.HappinessRateInSeconds, amount);
    }
}