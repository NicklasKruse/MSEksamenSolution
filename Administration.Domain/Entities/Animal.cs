using Administration.Domain.DomainServices;
using Administration.Domain.Enums;
using Administration.Domain.ValueObjects;
using CommonAssets;

namespace Administration.Domain.Entities
{

    public class Animal : Entity
    {
        public string Name { get; init; } // init gør den immutable og readonly
        public Weight? Weight { get; private set; }
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
        /// <summary>
        /// Sætter vægtklassen for dyret baseret på vægt og kategori 
        /// </summary>
        /// <param name="speciesService"></param>
        /// <exception cref="ArgumentException"></exception>
        private void SetWeightClass(ISpeciesService speciesService)
        {
            var species = speciesService.GetSpecies(SpeciesId.Value);

            var (min, max) = Category switch // Pattern matching switch expression
            {
                Category.Type_A => (species.IdealWeight.Min, species.IdealWeight.Max),
                Category.Type_B => (species.IdealWeight.Min, species.IdealWeight.Max),
                _ => throw new ArgumentException("Invalid category") // _ kaldes discard pattern 

            };

            if (Weight.Value <= 0)
            {
                throw new ArgumentException("Weight must be positive");
            }

            WeightClass = Weight.Value switch
            {
                var w when w < min => WeightClass.Light,
                var w when w > max => WeightClass.Heavy,
                _ => WeightClass.Medium // når ingen af de to ovenstående er true
            };
        }
    }
}
