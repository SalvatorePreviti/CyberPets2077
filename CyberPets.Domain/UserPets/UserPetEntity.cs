using System;
using CyberPets.Domain.PetKinds;

namespace CyberPets.Domain.UserPets
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
            HungerMetric = hungerMetric ?? new UserPetMetricValue(creationDate);
            HappinessMetric = happinessMetric ?? new UserPetMetricValue(creationDate);
        }

        public int GetHunger(DateTime now) => HungerMetric.GetValue(now, -Kind.HungerRateInSeconds);

        public UserPetMetricValue UpdatedHunger(DateTime now, double amount) => HungerMetric.Updated(now, -Kind.HungerRateInSeconds, -amount);

        public int GetHappiness(DateTime now) => HappinessMetric.GetValue(now, Kind.HappinessRateInSeconds);

        public UserPetMetricValue UpdatedHappiness(DateTime now, double amount) => HappinessMetric.Updated(now, Kind.HappinessRateInSeconds, amount);
    }
}