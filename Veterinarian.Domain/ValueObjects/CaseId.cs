namespace Veterinarian.Domain.ValueObjects
{
    public record CaseId
    {
        public Guid Value { get; init; }
        public CaseId(Guid value)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException("CaseId cannot be empty");
            }
            Value = value;
        }
        // Caster fra Guid til CaseId
        public static implicit operator CaseId(Guid value) => new CaseId(value);
    }
}
