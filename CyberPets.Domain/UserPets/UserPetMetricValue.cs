using System;

public struct UserPetMetricValue
{
    public const int MinValue = -10;

    public const int DefaultValue = 0;

    public const int MaxValue = +10;

    public DateTime LastUpdate { get; }

    public int LastValue { get; }

    public UserPetMetricValue(DateTime lastUpdate, int lastValue = DefaultValue)
    {
        LastUpdate = lastUpdate;
        LastValue = Math.Clamp(lastValue, MinValue, MaxValue);
    }

    public readonly int GetValue(DateTime now, double rateInSeonds)
    {
        return Math.Clamp(LastValue + (int)((LastUpdate - now).TotalSeconds / rateInSeonds), MinValue, MaxValue);
    }

    public readonly UserPetMetricValue Updated(DateTime now, double rateInSeconds, int amount)
    {
        return new UserPetMetricValue(now, GetValue(now, rateInSeconds) + amount);
    }
}