using Administration.Domain.Entities;
using Dapr.Client;
using Dapr.Workflow;
using System.Reactive;


namespace Administration.Api.Workflows
{
    public class CompensateEventPublishActivity : WorkflowActivity<Animal, Unit>
    {
        private readonly DaprClient _daprClient;

        public CompensateEventPublishActivity(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public override async Task<Unit> RunAsync(WorkflowActivityContext context, Animal animal)
        {
            // animal-created-cancelled er ikke lavet. Den skal fungere som en compensating event til animal-created

            //await _daprClient.PublishEventAsync(
            //    "pubsub",
            //    "animal-creation-cancelled",
            //    new { AnimalId = animal.Id });
            return Unit.Default;
        }
    }
}


