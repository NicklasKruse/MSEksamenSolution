using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Administration.Domain.ValueObjects;
using CommonAssets;

namespace Administration.Domain.Entities
{
    public class Species : Entity
    {
        public string Name { get; init; }
        public WeightRange IdealWeight { get; init; }

        public Species(Guid id,string name, WeightRange idealWeight)
        {
            Id = id;
            Name = name;
            IdealWeight = idealWeight;
        }
    }
}
