namespace CommonAssets.EventDtos
{
    // Det her var et forøg på at skabe en DTO, som Veterinarian.Api kunne bruge til at modtage data fra Administration.Api.
    public class AnimalCreatedEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        // SpeciesId??
    }
}
