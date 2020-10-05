using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberPets.Domain.PetKinds
{
    /// <summary>
    /// Keep the list of supported pet kinds and allow finding them by name.
    /// </summary>
    public class PetKindsList
    {
        private readonly IReadOnlyDictionary<string, PetKind> _byName;

        public PetKindsList(IEnumerable<PetKind> kinds)
        {
            _byName = new Dictionary<string, PetKind>(kinds.Select(kind => KeyValuePair.Create(kind.Name, kind)), StringComparer.InvariantCultureIgnoreCase);
        }

        public PetKind? GetByName(string? name) =>
            name != null ? _byName.GetValueOrDefault(name) : null;

        public IEnumerable<PetKind> Values { get => _byName.Values; }
    }
}