using System;

namespace CyberPets.API.Services
{
    public class SystemTimeProvider : ITimeProvider
    {
        public DateTime Now() => DateTime.Now;
    }
}