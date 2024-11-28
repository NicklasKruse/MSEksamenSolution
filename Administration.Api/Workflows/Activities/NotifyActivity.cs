using Administration.Domain.ValueObjects;
using Dapr.Workflow;

namespace Administration.Api.Workflows.Activities
{
    public class NotifyActivity : WorkflowActivity<Notification, object>
    {
        public override async Task<object> RunAsync(WorkflowActivityContext context, Notification notification)
        {
            await Task.CompletedTask;
            return null;
        }
    }
}


