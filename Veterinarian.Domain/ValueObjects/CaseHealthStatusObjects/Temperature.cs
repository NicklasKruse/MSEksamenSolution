using CommonAssets.ResultPattern;

namespace Veterinarian.Domain.ValueObjects.CaseHealthStatusObjects
{
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
    public enum TemperatureUnit
    {
        Celsius,
        Fahrenheit
    }

}
