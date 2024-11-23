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
        public static CaseId Create(Guid value)
        {
            return new CaseId(value);
        }

        //private void ValidateCase(Guid value)
        //{
        //    if (caseService.GetCases(value) == null)
        //    {
        //        throw new ArgumentException("Case not found");
        //    }
        //}

        // Caster fra Guid til CaseId
        public static implicit operator CaseId(Guid value) => new CaseId(value);
    }
}
