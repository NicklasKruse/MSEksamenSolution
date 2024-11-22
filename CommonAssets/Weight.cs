using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonAssets
{
    public record Weight
    {
        public decimal Value { get; init; }
        public Weight(decimal value)
        {
            if (value < 0)
            {
                throw new ArgumentException("Weight cannot be negative");
            }
            Value = value;
        }
        // Caster fra decimal til Weight
        public static implicit operator Weight(decimal value) => new Weight(value);
    }

}
