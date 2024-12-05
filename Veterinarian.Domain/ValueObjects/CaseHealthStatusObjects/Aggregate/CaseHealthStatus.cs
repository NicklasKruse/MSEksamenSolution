using CommonAssets.ResultPattern;

namespace Veterinarian.Domain.ValueObjects.CaseHealthStatusObjects.Aggregate
{
    public record CaseHealthStatus
    {
        public Temperature CurrentTemperature { get; init; }
        public HeartRate HeartRate { get; init; }
        public RespiratoryRate RespiratoryRate { get; init; }
        public HealthCondition Condition { get; init; }
        public DateTime AssessmentTime { get; init; }
        public Text Notes { get; init; }

        private CaseHealthStatus(Temperature temperature, HeartRate heartRate,
            RespiratoryRate respiratoryRate, HealthCondition condition,
            DateTime assessmentTime, Text notes)
        {
            CurrentTemperature = temperature;
            HeartRate = heartRate;
            RespiratoryRate = respiratoryRate;
            Condition = condition;
            AssessmentTime = assessmentTime;
            Notes = notes;
        }

        /// <summary>
        /// Factory method til at lave et CaseHealthStatus object / Fra DDD Eric Evans
        /// </summary>
        /// <param name="temperature"></param>
        /// <param name="heartRate"></param>
        /// <param name="respiratoryRate"></param>
        /// <param name="condition"></param>
        /// <param name="assessmentTime"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static Result<CaseHealthStatus> Create(
            Temperature temperature,
            HeartRate heartRate,
            RespiratoryRate respiratoryRate,
            HealthCondition condition,
            DateTime assessmentTime,
            Text notes)
        {
            if (assessmentTime > DateTime.UtcNow)
                return Result.Failure<CaseHealthStatus>("Assessment time cannot be in the future");

            return Result.Success(new CaseHealthStatus(
                temperature,
                heartRate,
                respiratoryRate,
                condition,
                assessmentTime,
                notes));
        }
    }
    public enum HealthCondition
    {
        Critical,
        Serious,
        Fair,
        Good,
        Excellent
    }
}
