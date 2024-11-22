using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veterinarian.Domain.ValueObjects
{
    public record MedicineId
    {
        public Guid Value { get; set; }

        public MedicineId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("MedicineId cannot be empty");
            }
            Value = value;
        }
        //public static implicit operator MedicineId(Guid value)
        //{
        //    return new MedicineId(value);
        //}
    }
}
