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

            var stateKey = $"animal-creation-{animal.Id}";

            var state = await _daprClient.GetStateAsync<ActivityState>("statestore", stateKey);
            if (state?.IsCompleted == true)
            {
                return animal;
            }

            try {
                _logger.LogInformation("Påbegyndt oprettelse af animal ID: {AnimalId}", animal.Id);

                var createdAnimal = await _repository.CreateAsync(animal);

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
                _logger.LogError(ex, "Kunne ikke oprette Animal: {AnimalId}", animal.Id);
                throw;
            }
        }
    }
}
