using System;
using CyberPets.Domain.Time;

namespace CyberPets.Test
{
    public class TestTimeProvider : ITimeProvider
    {
        public DateTime Value { get; set; }

        public DateTime Now() => Value;

        public TestTimeProvider(DateTime? value = null)
        {
            this.Value = value ?? DateTime.Now;
        }
    }
}