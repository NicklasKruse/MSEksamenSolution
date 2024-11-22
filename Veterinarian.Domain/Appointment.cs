using CommonAssets;
using Veterinarian.Domain.ValueObjects;

namespace Veterinarian.Domain
{
    public class Appointment : AggregateRoot
    {
        public Text Diagnosis { get; private set; }
        public Text Treatment { get; private set; }
    }
}
