using System.Threading.Tasks;
using CyberPets.Domain.PetKinds;
using Xunit;

namespace CyberPets.Test.Domain
{
    public class PetKinds_Test
    {
        [Fact]
        public void Can_create_and_get_values()
        {
            var animal1 = new PetKind("Animal1", 1, 1);
            var animal2 = new PetKind("Animal2", 2, 2);
            var kinds = new PetKindsList(new[] { animal1, animal2 });

            Assert.Same(animal1, kinds.GetByName(animal1.Name));
            Assert.Same(animal2, kinds.GetByName(animal2.Name));

            Assert.Contains(animal1, kinds.Values);
            Assert.Contains(animal2, kinds.Values);
        }

        [Fact]
        public void Get_by_name_is_case_insensitive()
        {
            var animal1 = new PetKind("Animal1", 1, 1);
            var animal2 = new PetKind("Animal2", 2, 2);
            var kinds = new PetKindsList(new[] { animal1, animal2 });

            Assert.Same(animal1, kinds.GetByName("animal1"));
            Assert.Same(animal2, kinds.GetByName("ANIMAL2"));
        }

        [Fact]
        public void Get_by_name_returns_null_if_not_found()
        {
            var kinds = new PetKindsList(new[] { new PetKind("AnimalX", 1, 1) });
            Assert.Null(kinds.GetByName("invalid"));
        }

        /// <summary>
        /// This is a regression test.
        /// We want to be sure that we don't delete by mistake an animal used by somebody and alreay stored in a database.
        /// </summary>
        [Fact]
        public void Regression_test_known_kinds()
        {
            var kinds = new PetKindsList(PetKind.KnownKinds);
            Assert.Equal(PetKind.Rabbit, kinds.GetByName("rabbit"));
            Assert.Equal(PetKind.Dog, kinds.GetByName("dog"));
            Assert.Equal(PetKind.Cat, kinds.GetByName("cat"));
            Assert.Equal(PetKind.Dragon, kinds.GetByName("dragon"));
        }
    }
}