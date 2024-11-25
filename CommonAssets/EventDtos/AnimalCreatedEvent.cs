namespace CommonAssets.EventDtos
{
    public class AnimalCreatedEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        // SpeciesId??
    }
}
