using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veterinarian.Domain.ValueObjects
{
    public record MedicineDosage
    {
        public decimal Quantity { get; init; }
        public UnitType Unit { get; init; }

        public MedicineDosage(decimal quantity, UnitType unit)
        {
            Quantity = quantity;
            Unit = unit;
        }
        public enum UnitType
        {
            Milligram,
            Gram,
            Milliliter,
            Liter,
            Drop,
            Tablet,
            Capsule
        }
    }
}
