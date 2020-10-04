using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberPets.Domain
{
    public class PetKinds
    {
        private IReadOnlyDictionary<string, PetKind> _byName;

        public PetKinds(IEnumerable<PetKind> kinds)
        {
            _byName = new Dictionary<string, PetKind>(kinds.Select(kind => KeyValuePair.Create(kind.Name, kind)));
        }

        public PetKind GetByName(string name) => _byName.GetValueOrDefault(name);

        public IEnumerable<PetKind> Values { get => _byName.Values; }
    }
}