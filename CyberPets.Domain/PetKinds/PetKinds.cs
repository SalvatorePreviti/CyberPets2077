using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberPets.Domain.PetKinds
{
    public class PetKinds
    {
        private readonly IReadOnlyDictionary<string, PetKind> _byName;

        public PetKinds(IEnumerable<PetKind> kinds)
        {
            _byName = new Dictionary<string, PetKind>(kinds.Select(kind => KeyValuePair.Create(kind.Name, kind)), StringComparer.InvariantCultureIgnoreCase);
        }

        public PetKind? GetByName(string? name) =>
            name != null ? _byName.GetValueOrDefault(name) : null;

        public IEnumerable<PetKind> Values { get => _byName.Values; }
    }
}