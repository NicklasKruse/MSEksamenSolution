using CommonAssets;
using Veterinarian.Domain.ValueObjects;

namespace Veterinarian.Domain
{
    public class MedicineDispencer : Entity //Vi vil gerne kunne identificere de enkelte dispencere, som så holder på nogle value objects
    {
        public MedicineId MedicineId { get; init; }
        public MedicineDosage Dosage { get; init; }

        public MedicineDispencer(MedicineId medicineId, MedicineDosage dosage)
        {
            //Entity Id til base class
            Id = Guid.NewGuid();
            MedicineId = medicineId;
            Dosage = dosage;
        }
    }
}
