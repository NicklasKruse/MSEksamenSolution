using CommonAssets;
using CommonAssets.ResultPattern;
using Veterinarian.Domain.ValueObjects;
using Veterinarian.Domain.ValueObjects.CaseHealthStatusObjects;

namespace Veterinarian.Domain
{
    public class Appointment : AggregateRoot
    {
        private readonly List<MedicineDispencer> medicineAdministered = new();
        private readonly List<CaseHealthStatus> caseHealthStatusReadings = new();

        public DateTime CreatedAtTime { get; init; }
        public DateTime? CompletedAtTime { get; private set; }
        public Text Diagnosis { get; private set; }
        public Text Treatment { get; private set; }
        public CaseId CaseId { get; init; }
        public Weight RecordedWeight { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public IReadOnlyList<MedicineDispencer> MedicineAdministered => medicineAdministered; //For ikke at skulle expose min liste direkte. Kan man nu tilgå listen igennem denne property
        public IReadOnlyList<CaseHealthStatus> CaseHealthStatusReadings => caseHealthStatusReadings; // Samme som ovenstående
        public Appointment(CaseId caseId)
        {
            Id = Guid.NewGuid();
            CaseId = caseId;
            Status = AppointmentStatus.Active;
            CreatedAtTime = DateTime.UtcNow;
        }
        /// <summary>
        /// IEnumerable så vi kan modtage flere status checks over tid, og ikke kun en ad gangen
        /// </summary>
        /// <param name="caseHealthStatuses"></param>
        public Result RegisterCaseHealthStatus(IEnumerable<CaseHealthStatus> caseHealthStatuses)
        {
            ValidateAppointment();

            var validationResults = new List<string>();
            var validReadings = new List<CaseHealthStatus>();

            foreach(var caseHealthStatus in caseHealthStatuses)
            {
                var result = CaseHealthStatus.Create(
                    caseHealthStatus.CurrentTemperature,
                    caseHealthStatus.HeartRate,
                    caseHealthStatus.RespiratoryRate,
                    caseHealthStatus.Condition,
                    caseHealthStatus.AssessmentTime,
                    caseHealthStatus.Notes);
                if(result.IsSuccess)
                {
                    validReadings.Add(result.Value);
                }
                else
                {
                    validationResults.Add(result.Error);
                }
            }

            caseHealthStatusReadings.AddRange(caseHealthStatuses);
            return Result.Success();
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
