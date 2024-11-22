using Administration.Domain.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Administration.Domain.ValueObjects
{
    public record SpeciesId
    {
        private readonly ISpeciesService speciesService;
        public Guid Value { get; init; }

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
