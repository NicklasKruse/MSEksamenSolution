using CommonAssets.ResultPattern;

namespace Veterinarian.Domain.ValueObjects.CaseHealthStatusObjects
{
    public record HeartRate
    {
        public int BeatsPerMinute { get; init; }

        private HeartRate(int bpm)
        {
            BeatsPerMinute = bpm;
        }

        public static Result<HeartRate> Create(int bpm)
        {
            if (bpm < 20 || bpm > 300)
                return Result.Failure<HeartRate>("Heart rate is outside valid range");

            return Result.Success(new HeartRate(bpm));
        }
    }
}
