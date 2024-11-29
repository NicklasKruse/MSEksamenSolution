using Administration.Domain.ValueObjects;
using Dapr.Client;
using Dapr.Workflow;
using System.Reactive;

namespace Administration.Api.Workflows.Activities
{
    //public class NotifyActivity : WorkflowActivity<Notification, object>
    //{
    //    public override async Task<object> RunAsync(WorkflowActivityContext context, Notification notification)
    //    {
    //        await Task.CompletedTask;
    //        return null;
    //    }
    //}
    public class NotifyActivity : WorkflowActivity<Domain.ValueObjects.Notification, Unit>
    {
        private readonly DaprClient _daprClient;

        public NotifyActivity(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public override async Task<Unit> RunAsync(WorkflowActivityContext context, Domain.ValueObjects.Notification notification)
        {
            var notificationEvent = new
            {
                Message = notification.Message,
                AnimalId = notification.Animal.Id,
                AnimalName = notification.Animal.Name,
                Timestamp = DateTime.UtcNow
            };

            await _daprClient.PublishEventAsync(
                "pubsub",
                "animal-created",
                notificationEvent);

            return Unit.Default;
        }
    }

}


