using System;

namespace CyberPets.Domain.Time
{
    public interface ITimeProvider
    {
        DateTime Now();
    }
}