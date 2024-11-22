using Administration.Domain.DomainServices;
using Administration.Domain.ValueObjects;

namespace Administration.Domain.Entities
{

    public class Animal : Entity
    {
        public string Name { get; init; } // init gør den immutable og readonly
        public Weight Weight { get; private set; }
        public WeightClass WeightClass { get; private set; }
        public Category Category { get; set; }

        public SpeciesId SpeciesId { get; set; }


        public Animal(Guid id, string name, Category Category, SpeciesId speciesId)
        {
            Id = id;
            Name = name;
            Category = Category;
            SpeciesId = speciesId;
        }
        public void SetWeight(Weight weight)
        {
            Weight = weight;

        }
        private void SetWeightClass(ISpeciesService speciesService)
        {
            var species = speciesService.GetSpecies(SpeciesId.Value);


        }
    }
   
    public enum Category
    {
        Type_A,
        Type_B,
    }
    public enum WeightClass
    {
        Light,
        Medium,
        Heavy
    }
}
