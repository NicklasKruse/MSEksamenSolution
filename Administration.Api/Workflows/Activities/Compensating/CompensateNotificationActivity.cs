using Administration.Domain.Entities;
using Dapr.Workflow;
using System.Reactive;


namespace Administration.Api.Workflows
{
    /// <summary>
    /// Activity til at sende en notifikation angående compensating transaction behov
    /// </summary>
    public class CompensateNotificationActivity : WorkflowActivity<Animal, Unit>
    {
        public override async Task<Unit> RunAsync(WorkflowActivityContext context, Animal animal)
        {
            await Task.CompletedTask;
            return Unit.Default;
        }
    }
}


