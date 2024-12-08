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
            var stateKey = $"event-publish-{eventData.Id}";

            var state = await _daprClient.GetStateAsync<ActivityState>("statestore", stateKey);
            if (state?.IsCompleted == true)
            {
                return Unit.Default;
            }

            await _daprClient.PublishEventAsync("pubsub", "animal-created", eventData);

            await _daprClient.SaveStateAsync(
           "statestore",
           stateKey,
           new ActivityState { IsCompleted = true, Timestamp = DateTime.UtcNow });

            return Unit.Default;
        }
    }
}


