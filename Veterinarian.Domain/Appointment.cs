using CommonAssets;
using Veterinarian.Domain.ValueObjects;

namespace Veterinarian.Domain
{
    public class Appointment : AggregateRoot
    {
        private readonly List<MedicineDispencer> medicineAdministered = new();

        public DateTime CreatedAtTime { get; init; }
        public DateTime? CompletedAtTime { get; private set; }
        public Text Diagnosis { get; private set; }
        public Text Treatment { get; private set; }
        public CaseId CaseId { get; init; }
        public Weight RecordedWeight { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public IReadOnlyList<MedicineDispencer> MedicineAdministered => medicineAdministered; //For ikke at skulle expose min liste direkte. Kan man nu tilgå listen igennem denne property
        public Appointment(CaseId caseId)
        {
            Id = Guid.NewGuid();
            CaseId = caseId;
            Status = AppointmentStatus.Active;
            CreatedAtTime = DateTime.UtcNow;
        }

        public void DispenceMedicine(MedicineId medicineId, MedicineDosage medicineDosage)
        {
            ValidateAppointment();

            var medicineDispencer = new MedicineDispencer(medicineId, medicineDosage);
            medicineAdministered.Add(medicineDispencer);
        }
        
        public void SetCurrentWeight(Weight weight)
        {
            ValidateAppointment();
            RecordedWeight = weight;
        }
        public void UpdateDiagnosis(Text diagnosis)
        {
            //Kan ikke ændre diagnose på en afsluttet eller aflyst aftale
            ValidateAppointment();
            Diagnosis = diagnosis;
        }
        public void UpdateTreatment(Text treatment)
        {
            //Kan ikke ændre behandling/Treatment på en afsluttet eller aflyst aftale
            ValidateAppointment();
            Treatment = treatment;
        }


        #region Set Appointment Status Methods
        public void CompleteAppointment()
        {
            //Kan ikke afslutte en afsluttet eller aflyst aftale
            ValidateAppointment();

            if (Diagnosis == null)
                throw new InvalidOperationException("Diagnosis must be set before completing the appointment");
            if (Treatment == null)
                throw new InvalidOperationException("Treatment must be set before completing the appointment");

            Status = AppointmentStatus.Completed;
            CompletedAtTime = DateTime.UtcNow;
        }
        public void CancelAppointment()
        {
            //Kan ikke aflyse en afsluttet eller aflyst aftale
            ValidateAppointment();
            Status = AppointmentStatus.Cancelled;
        }

        /// <summary>
        /// Validering af Appointment aggregat
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void ValidateAppointment()
        {
            if (Status == AppointmentStatus.Completed)
            {
                throw new InvalidOperationException("Appointment already completed");
                //throw new DomainException("Cannot update a completed appointment");
            }
            if (Status == AppointmentStatus.Cancelled)
            {
                throw new InvalidOperationException("Appointment has been cancelled");
                //throw new DomainException("Cannot update a cancelled appointment");
            }
            if (Status == AppointmentStatus.Pending)
            {
                throw new InvalidOperationException("Appointment is pending approval, can not set properties unless the Satus is Active");
                //throw new DomainException("Cannot update a pending appointment");
            }
        }
        #endregion
    }

    #region ValueObjects
    /// <summary>
    /// Disse ord er en del af vores ubiquitous language. Det er ord med fælles forståelse mellem udviklere og forretning.
    /// </summary>
    public enum AppointmentStatus
    {
        Pending,
        Active,
        Completed,
        Cancelled
    }
    #endregion
}
