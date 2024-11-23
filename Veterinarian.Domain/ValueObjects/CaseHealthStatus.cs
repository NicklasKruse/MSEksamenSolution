using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veterinarian.Domain.ValueObjects
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

    public record Temperature
    {
        public decimal Value { get; init; }
        public TemperatureUnit Unit { get; init; }

        private Temperature(decimal value, TemperatureUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        public static Result<Temperature> Create(decimal value, TemperatureUnit unit)
        {
            if (value < 20 || value > 45) // Celsius range
                return Result.Failure<Temperature>("Temperature is outside valid range");

            return Result.Success(new Temperature(value, unit));
        }
    }

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

    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit
    }

}
