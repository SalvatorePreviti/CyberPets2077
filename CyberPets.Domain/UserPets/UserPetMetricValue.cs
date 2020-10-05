using System;

namespace CyberPets.Domain.UserPets
{
    public struct UserPetMetricValue
    {
        public const int MinValue = -20;

        public const int NeutralValue = 0;

        public const int MaxValue = +20;

        public DateTime LastUpdate { get; }

        public double LastValue { get; }

        public UserPetMetricValue(DateTime lastUpdate, double lastValue = NeutralValue)
        {
            LastUpdate = lastUpdate;
            LastValue = Math.Clamp(lastValue, MinValue, MaxValue);
        }

        public readonly int GetValue(DateTime now, double rateInSeonds) =>
            (int)Math.Clamp(LastValue + ((LastUpdate - now).TotalSeconds / rateInSeonds), MinValue, MaxValue);

        public readonly UserPetMetricValue Updated(DateTime now, double rateInSeconds, double amount) =>
            new UserPetMetricValue(now, GetValue(now, rateInSeconds) + amount);
    }
}