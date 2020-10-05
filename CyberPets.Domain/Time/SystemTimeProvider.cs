using System;

namespace CyberPets.Domain.Time
{
    public class SystemTimeProvider : ITimeProvider
    {
        public DateTime Now() => DateTime.Now;
    }
}