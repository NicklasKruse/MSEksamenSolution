using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Administration.Domain
{
    public class Species : Entity
    {
        public string Name { get; set; }
        public WeightRange IdealWeight { get; set; }

        public Species( string name, WeightRange idealWeight)
        {
            Name = name;
            IdealWeight = idealWeight;
        }
    }
