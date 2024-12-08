using Administration.Domain.ValueObjects;
using CommonAssets.EventDtos;
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
    /// <summary>
    /// Ikke i brug!
    /// </summary>
    public class NotifyActivity : WorkflowActivity<Domain.ValueObjects.Notification, Unit>
    {
        private readonly DaprClient _daprClient;
        private readonly string STATE_STORE = "statestore";

        public NotifyActivity(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public override async Task<Unit> RunAsync(WorkflowActivityContext context, Domain.ValueObjects.Notification notification)
        {

            var stateKey = $"notification-{notification.Animal.Id}";

            var state = await _daprClient.GetStateAsync<NotificationState>("statestore", stateKey);
            if (state?.Status == "Completed")
            {
                return Unit.Default;
            }

            try {

                await _daprClient.SaveStateAsync(
                 "statestore",
                 stateKey,
                 new NotificationState { Status = "Started" });

                var notificationEvent = new
            {
                AnimalId = notification.Animal.Id,
                AnimalName = notification.Animal.Name,
                Timestamp = DateTime.UtcNow
            };

            await _daprClient.PublishEventAsync(
                "pubsub",
                "animal-created",
                notificationEvent);

            await _daprClient.SaveStateAsync(
                 STATE_STORE,
                 $"notification-{notification.Animal.Id}",
                 new NotificationState { Status = "Completed" });

            return Unit.Default;
            }
            catch (Exception ex)
            {
                await _daprClient.SaveStateAsync(
                    STATE_STORE,
                    $"notification-{notification.Animal.Id}",
                    new NotificationState { Status = "Failed" });
                throw ex;
            }
        }
    }

}


