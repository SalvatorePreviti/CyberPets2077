using System;

public struct UserPetMetricValue
{
    public const double MinValue = -20;

    public const double DefaultValue = 0;

    public const double MaxValue = +20;

    public DateTime LastUpdate { get; }

    public double LastValue { get; }

    public UserPetMetricValue(DateTime lastUpdate, double lastValue = DefaultValue)
    {
        LastUpdate = lastUpdate;
        LastValue = Math.Clamp(lastValue, MinValue, MaxValue);
    }

    public readonly double GetValue(DateTime now, double rateInSeonds) =>
        Math.Clamp(LastValue + (int)((LastUpdate - now).TotalSeconds / rateInSeonds), MinValue, MaxValue);

    public readonly UserPetMetricValue Updated(DateTime now, double rateInSeconds, double amount) =>
        new UserPetMetricValue(now, GetValue(now, rateInSeconds) + amount);
}