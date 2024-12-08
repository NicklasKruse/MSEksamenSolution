using Administration.Domain.Entities;
using Administration.Domain.ValueObjects;
using CommonAssets;
using CommonAssets.EventDtos;
using Dapr.Client;
using Dapr.Workflow;

namespace Administration.Api.Workflows.Activities
{
    public class CreateAnimalActivity : WorkflowActivity<Animal, Animal>
    {
        private readonly DaprClient _daprClient;
        private readonly IRepositoryEx _repository;
        private readonly ILogger<CreateAnimalActivity> _logger;

        public CreateAnimalActivity(DaprClient daprClient, IRepositoryEx repository, ILogger<CreateAnimalActivity> logger)
        {
            _daprClient = daprClient;
            _repository = repository;
            _logger = logger;
        }

        public override async Task<Animal> RunAsync(WorkflowActivityContext context, Animal animal)
        {
            try {
                _logger.LogInformation("Påbegyndt oprettelse af animal ID: {AnimalId}", animal.Id);

                var existing = await _repository.GetByIdAsync(animal.Id);
                if (existing != null)
                {
                    _logger.LogWarning("Animal eksisterer allerede {AnimalId} ", animal.Id);
                }

                var createdAnimal = await _repository.CreateAsync(animal);

                await _daprClient.PublishEventAsync(
               "pubsub", // subscription.yaml pubsub name
               "animal-created", // Topic
               new AnimalCreatedEvent
               {
                   Id = createdAnimal.Id,
                   Name = createdAnimal.Name
               });

                await _daprClient.SaveStateAsync(
                "statestore",
                $"animal-creation-{animal.Id}", new ActivityState
                {
                    IsCompleted = true,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("Animal oprettet {AnimalId}", animal.Id);
                return createdAnimal;
            }
            catch (Exception ex)
            {
                await _daprClient.PublishEventAsync(
                "pubsub",
                "animal-creation-cancelled",
                new
                {
                    AnimalId = animal.Id,
                    Timestamp = DateTime.UtcNow,
                    Error = ex.Message
                });
                throw;
            }
        }
    }
}
