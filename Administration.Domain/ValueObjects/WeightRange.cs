namespace Administration.Domain.ValueObjects
{
    public record WeightRange
    {
        public decimal Min { get; init; }
        public decimal Max { get; init; }
        public WeightRange(decimal min, decimal max)
        {
            if (min < 0 || max < 0)
            {
                throw new ArgumentException("Weight cannot be negative");
            }
            if (min > max)
            {
                throw new ArgumentException("Min weight cannot be greater than max weight");
            }
            Min = min;
            Max = max;
        }
    }
}
