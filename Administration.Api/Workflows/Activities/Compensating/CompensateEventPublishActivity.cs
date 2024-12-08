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
                var stateKey = $"compensation-event-{animal.Id}";
                var state = await _daprClient.GetStateAsync<CompensationState>("statestore", stateKey);

                if (state?.IsCompleted == true)
                {
                    return Unit.Default;
                }

                // Publish event til pubsub med information om at animal creation er blevet annulleret
                await _daprClient.PublishEventAsync(
                    "pubsub",
                    "animal-creation-cancelled",
                    new
                    {
                        AnimalId = animal.Id,
                        Timestamp = DateTime.UtcNow
                    });

                await _daprClient.SaveStateAsync(
                    "statestore",
                    stateKey,
                    new CompensationState
                    {
                        IsCompleted = true,
                        Timestamp = DateTime.UtcNow
                    });

                _logger.LogInformation("Compensation event published for {AnimalId}", animal.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Publish compensation event fejlede: {AnimalId}", animal.Id);
                throw;
            }

            return Unit.Default;
        }
    }
}


