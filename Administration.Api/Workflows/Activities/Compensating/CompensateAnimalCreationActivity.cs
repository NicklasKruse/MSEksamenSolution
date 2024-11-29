using Administration.Domain.Entities;
using Dapr.Client;
using Dapr.Workflow;
using System.Reactive;


namespace Administration.Api.Workflows
{
    public class CompensateAnimalCreationActivity : WorkflowActivity<Animal, Unit>
    {
        private readonly DaprClient _daprClient;

        public CompensateAnimalCreationActivity(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public override async Task<Unit> RunAsync(WorkflowActivityContext context, Animal animal)
        {
            // Her kunne et event sendes tilbage til pubsub, som kunne fortælle at en animal er blevet slettet
            await Task.CompletedTask;
            return Unit.Default;
        }
    }
}


