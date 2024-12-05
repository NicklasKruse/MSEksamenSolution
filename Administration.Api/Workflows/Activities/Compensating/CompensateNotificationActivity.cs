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
        private readonly ILogger<CompensateNotificationActivity> _logger;

        public CompensateNotificationActivity(ILogger<CompensateNotificationActivity> logger)
        {
            _logger = logger;
        }

        public override async Task<Unit> RunAsync(WorkflowActivityContext context, Animal animal)
        {
            try
            {
                _logger.LogInformation("notifikation sendt {}", animal.Id);
                // Notifikations logik ikke implementeret

                await Task.CompletedTask; // Placeholder for actual implementation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "afsendelse af notifikation fejlede {}", animal.Id);
                throw; 
            }

            return Unit.Default;
        }
    }
}


