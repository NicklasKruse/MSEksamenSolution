using Administration.Domain.Entities;
using Dapr.Client;
using Dapr.Workflow;
using System.Reactive;


namespace Administration.Api.Workflows
{
    public class CompensateEventPublishActivity : WorkflowActivity<Animal, Unit>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<CompensateEventPublishActivity> _logger;

        public CompensateEventPublishActivity(DaprClient daprClient, ILogger<CompensateEventPublishActivity> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        public override async Task<Unit> RunAsync(WorkflowActivityContext context, Animal animal)
        {
            try
            {
                await _daprClient.PublishEventAsync(
                    "pubsub",
                    "animal-creation-cancelled", // det er en compensating på animal-created. Så her har vi en til hvis den er cancelled af en eller anden grund.
                    new { AnimalId = animal.Id, Timestamp = DateTime.UtcNow });

                _logger.LogInformation("Compensating transaction fuldendt på {AnimalId}", animal.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction fejlede animal: {AnimalId}", animal.Id);
                throw; 
            }

            return Unit.Default;
        }
    }
}


