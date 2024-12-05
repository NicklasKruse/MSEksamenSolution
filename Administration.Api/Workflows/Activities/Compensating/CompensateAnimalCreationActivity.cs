using Administration.Domain.Entities;
using Dapr.Client;
using Dapr.Workflow;
using System.Reactive;


namespace Administration.Api.Workflows
{
    public class CompensateAnimalCreationActivity : WorkflowActivity<Animal, Unit>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<CompensateAnimalCreationActivity> _logger;

        public CompensateAnimalCreationActivity(DaprClient daprClient, ILogger<CompensateAnimalCreationActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<Unit> RunAsync(WorkflowActivityContext context, Animal animal)
        {
            try
            {
                // sletter animal. Her skal vi have et tilsvarende topic for at det skal virke
                await _daprClient.PublishEventAsync(
                    "pubsub",
                    "animal-deleted",
                    new { AnimalId = animal.Id, Timestamp = DateTime.UtcNow });

                _logger.LogInformation("Comp. transactions fuldført {}", animal.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Comp. Transaction fejlede {}", animal.Id);
                throw; 
            }

            return Unit.Default;
        }
    }
}


