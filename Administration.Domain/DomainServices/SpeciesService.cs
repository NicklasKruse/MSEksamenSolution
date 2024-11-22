using Administration.Domain.Entities;

namespace Administration.Domain.DomainServices
{
    public class SpeciesService : ISpeciesService
    {

        private readonly List<Species> _species = new List<Species> //Placeholder data
        {
            new Species(Guid.NewGuid(), "Dog", new ValueObjects.WeightRange(1, 10)),
            new Species(Guid.NewGuid(), "Cat", new ValueObjects.WeightRange(1, 5)),
            new Species(Guid.NewGuid(), "Horse", new ValueObjects.WeightRange(100, 500))
        };

        public Species? GetSpecies(Guid speciesId)
        {
            if(speciesId == Guid.Empty)
            {
                throw new ArgumentException("SpeciesId cannot be empty");
            }
            var result = _species.FirstOrDefault(s => s.Id == speciesId);
            return result ?? throw new ArgumentException("Species not found");
        }
    }}
}
