using System;

namespace CyberPets.Domain.UserPets
{
    public struct UserPetMetricValue
    {
        public const int MinValue = -20;

        public const int NeutralValue = 0;

        public const int MaxValue = +20;

        public DateTime LastUpdate { get; }

        public int LastValue { get; }

        public UserPetMetricValue(DateTime lastUpdate, int lastValue = NeutralValue)
        {
            LastUpdate = lastUpdate;
            LastValue = Math.Clamp(lastValue, MinValue, MaxValue);
        }

        public readonly int GetValue(DateTime now, int rateInSeconds) =>
            Math.Clamp(LastValue + (int)((now - LastUpdate).TotalSeconds / rateInSeconds), MinValue, MaxValue);

        public readonly UserPetMetricValue Updated(DateTime now, int rateInSeconds, int amount) =>
            new UserPetMetricValue(now, GetValue(now, rateInSeconds) + amount);
    }
}