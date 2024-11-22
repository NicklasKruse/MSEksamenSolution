using Administration.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Administration.Domain.DomainServices
{
    public interface ISpeciesService
    {
        Species? GetSpecies(Guid speciesId);
    }
}
