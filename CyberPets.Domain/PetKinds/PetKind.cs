using System;
using System.Collections.Generic;

namespace CyberPets.Domain.PetKinds
{
    public class PetKind
    {
        public string Name { get; private set; }

        public double HungerRateInSeconds { get; private set; }

        public double HappinessRateInSeconds { get; private set; }

        public PetKind(string name, int hungerRateInSeconds, double happinessRateInSeconds)
        {
            if (String.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException(nameof(name)); }
            if (hungerRateInSeconds <= 0) { throw new ArgumentOutOfRangeException(nameof(hungerRateInSeconds)); }
            if (happinessRateInSeconds <= 0) { throw new ArgumentOutOfRangeException(nameof(happinessRateInSeconds)); }

            Name = name;
            HungerRateInSeconds = hungerRateInSeconds;
            HappinessRateInSeconds = happinessRateInSeconds;
        }

        public static readonly PetKind Rabbit = new PetKind(name: "Rabbit", hungerRateInSeconds: 10, happinessRateInSeconds: 10);
        public static readonly PetKind Cat = new PetKind(name: "Cat", hungerRateInSeconds: 3, happinessRateInSeconds: 8);
        public static readonly PetKind Dog = new PetKind(name: "Dog", hungerRateInSeconds: 4, happinessRateInSeconds: 4);
        public static readonly PetKind Dragon = new PetKind(name: "Dragon", hungerRateInSeconds: 1, happinessRateInSeconds: 20);

        public static readonly IReadOnlyList<PetKind> KnownKinds = new[] { Rabbit, Cat, Dog, Dragon };
    }
}