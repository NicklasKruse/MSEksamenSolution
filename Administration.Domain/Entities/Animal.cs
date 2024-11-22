using Administration.Domain.DomainServices;
using Administration.Domain.ValueObjects;

namespace Administration.Domain.Entities
{

    public class Animal : Entity
    {
        public string Name { get; init; } // init gør den immutable og readonly
        public Weight Weight { get; private set; }
        public WeightClass WeightClass { get; private set; }
        public EnumExample EnumExample { get; set; }

        public SpeciesId SpeciesId { get; set; }


        public Animal(Guid id, string name, EnumExample enumExample, SpeciesId speciesId)
        {
            Id = id;
            Name = name;
            EnumExample = enumExample;
            SpeciesId = speciesId;
        }
        public void SetWeight(Weight weight)
        {
            Weight = weight;

        }
        private void SetWeightClass(ISpeciesService speciesService)
        {
            var species = speciesService.GetSpecies(SpeciesId.Value);

            var (from, to) = EnumExample switch
            {
                EnumExample.EnumValue1 => (species.IdealWeight.From, species.IdealWeight.To),
                EnumExample.EnumValue2 => (species.IdealWeight.From * 2, species.IdealWeight.To * 2),
                _ => (0, 0)
            };
        }
    }
   
    public enum EnumExample
    {
        EnumValue1,
        EnumValue2,
    }
    public enum WeightClass
    {
        Light,
        Medium,
        Heavy
    }
}
