using CommonAssets.EventDtos;
using Dapr.Client;
using Dapr.Workflow;
using System.Reactive;

namespace Administration.Api.Workflows.Activities
{
    public class PublishEventActivity : WorkflowActivity<AnimalCreatedEvent, Unit>
    {
        private readonly DaprClient _daprClient;

        public PublishEventActivity(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public override async Task<Unit> RunAsync(WorkflowActivityContext context, AnimalCreatedEvent eventData)
        {
            await _daprClient.PublishEventAsync("pubsub", "animal-created", eventData);
            return Unit.Default;
        }
    }
}


