using System;

namespace CyberPets.API.Services
{
    public interface ITimeProvider
    {
        DateTime Now();
    }
}