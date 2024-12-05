using Administration.Domain.DomainServices;

namespace Administration.Domain.ValueObjects
{
    public record SpeciesId
    {
        private readonly ISpeciesService speciesService;

        public Guid Value { get; init; }

        private SpeciesId(Guid value)
        {
            Value = value;
        }
        /// <summary>
        /// Factory method 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SpeciesId Create(Guid value)
        {
            return new SpeciesId(value);
        }

        public SpeciesId(Guid value, ISpeciesService speciesService)
        {
            this.speciesService = speciesService;

            ValidateSpecies(value);

            Value = value;
        }

        private void ValidateSpecies(Guid value)
        {
            if(speciesService.GetSpecies(value) == null)
                {
                throw new ArgumentException("Species not found");
            }
        }
    }
}
