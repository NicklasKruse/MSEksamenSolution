using Administration.Api.Workflows.Activities;
using Administration.Domain.DomainServices;
using Administration.Domain.Entities;
using Administration.Domain.ValueObjects;
using CommonAssets;
using CommonAssets.EventDtos;
using Dapr.Workflow;


namespace Administration.Api.Workflows
{
    public class AnimalWorkflow : Workflow<Animal, AnimalCreatedEvent>
    {
        private readonly ISpeciesService SpeciesService;
        private readonly ILogger<AnimalWorkflow> _logger;
        public AnimalWorkflow(ISpeciesService speciesService, ILogger<AnimalWorkflow> logger)
        {
            SpeciesService = speciesService;
            _logger = logger;
        }

        public override async Task<AnimalCreatedEvent> RunAsync(WorkflowContext context, Animal animal)
        {
            var speciesId = new SpeciesId(Guid.NewGuid(), SpeciesService);

            var newAnimal = await context.CallActivityAsync<Animal>(
                nameof(CreateAnimalActivity),
                new CreateAnimalRequest(animal, speciesId));

            newAnimal.SetWeight(new Weight(animal.Weight.Value));

            var animalCreatedEvent = new AnimalCreatedEvent
            {
                Id = newAnimal.Id,
                Name = newAnimal.Name,
                //Category = newAnimal.Category,
                //Weight = newAnimal.Weight.Value,
                //SpeciesId = newAnimal.SpeciesId.Value
            };

            // Publish 
            await context.CallActivityAsync(
                nameof(PublishEventActivity),
                new CommonAssets.PublishEventRequest("pubsub", "animal-created", animalCreatedEvent));

            // notification
            await context.CallActivityAsync(
                nameof(NotifyActivity),
                new Notification("Completed Reg.", newAnimal));

            return animalCreatedEvent;
        }

        #region Placeholder til service registrering
        public AnimalWorkflow()
        {
            //Placeholder til service registrering
        }
        #endregion
    }
}


