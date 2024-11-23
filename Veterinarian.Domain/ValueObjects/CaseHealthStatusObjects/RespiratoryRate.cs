using CommonAssets.ResultPattern;

namespace Veterinarian.Domain.ValueObjects.CaseHealthStatusObjects
{
    public record RespiratoryRate
    {
        public int BreathsPerMinute { get; init; }

        private RespiratoryRate(int bpm)
        {
            BreathsPerMinute = bpm;
        }

        public static Result<RespiratoryRate> Create(int bpm)
        {
            if (bpm < 4 || bpm > 100)
                return Result.Failure<RespiratoryRate>("Respiratory rate is outside valid range");

            return Result.Success(new RespiratoryRate(bpm));
        }
    }
}
