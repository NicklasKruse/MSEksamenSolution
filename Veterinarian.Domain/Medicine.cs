using CommonAssets;

namespace Veterinarian.Domain
{
    public class Medicine : Entity // Medicinen må gerne kunne spores ved individuelle ID'er. Det er forretningskritisk at kunne spore medicin tilbage til en specifik batch.
    {
        public string Name { get; init; }
    }
}
