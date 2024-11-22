using CommonAssets;
using Veterinarian.Domain.ValueObjects;

namespace Veterinarian.Domain
{
    public class Appointment : AggregateRoot
    {
        public DateTime CreatedAtTime { get; init; }
        public DateTime? CompletedAtTime { get; private set; }
        public Text Diagnosis { get; private set; }
        public Text Treatment { get; private set; }
        public CaseId CaseId { get; init; }
        public Weight RecordedWeight { get; private set; }
        public AppointmentStatus Status { get; private set; }
        public Appointment(CaseId caseId)
        {
            Id = Guid.NewGuid();
            CaseId = caseId;
            Status = AppointmentStatus.Active;
            CreatedAtTime = DateTime.UtcNow;
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
        public void CompleteAppointment()
        {
            //Kan ikke afslutte en afsluttet eller aflyst aftale
            ValidateAppointment();

            if(Diagnosis == null)
                throw new InvalidOperationException("Diagnosis must be set before completing the appointment");
            if(Treatment == null)
                throw new InvalidOperationException("Treatment must be set before completing the appointment");

            Status = AppointmentStatus.Completed;
            CompletedAtTime = DateTime.UtcNow;
        }
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
    }
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
}
