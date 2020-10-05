using System;
using System.Threading.Tasks;
using CyberPets.Domain.Time;
using Xunit;

namespace CyberPets.Test.Domain
{
    public class SystemTimeProvider_Test
    {
        [Fact]
        public void Now_returns_the_system_time()
        {
            var sut = new SystemTimeProvider();
            Assert.True(sut.Now() <= DateTime.Now);
            Assert.True((DateTime.Now - sut.Now()).TotalMinutes < 1);
        }
    }
}