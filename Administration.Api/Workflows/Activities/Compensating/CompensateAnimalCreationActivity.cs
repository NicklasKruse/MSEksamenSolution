using Administration.Domain.Entities;
using Dapr.Client;
using Dapr.Workflow;
using System.Reactive;


namespace Administration.Api.Workflows
{
    public class CompensateAnimalCreationActivity : WorkflowActivity<Animal, Unit>
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<CompensateAnimalCreationActivity> _logger;
        private readonly IRepositoryEx _repository; 

        public CompensateAnimalCreationActivity(DaprClient daprClient, ILogger<CompensateAnimalCreationActivity> logger, IRepositoryEx repository)
        {
            _daprClient = daprClient;
            _logger = logger;
            _repository = repository;
        }

        public override async Task<Unit> RunAsync(WorkflowActivityContext context, Animal animal)
        {
            try
            {
                // vi bruger en statestore til at holde styr på om vi har fuldført kompensationen
                var stateKey = $"compensation-animal-creation-{animal.Id}"; //giv den et unikt navn vha. animal.Id. 
                var state = await _daprClient.GetStateAsync<CompensationState>(
                    "statestore",
                    stateKey); 
                // her tjekker vi om kompensationen allerede er fuldført
                if (state?.IsCompleted == true)
                {
                    _logger.LogInformation("Compensation allerede fuldført {}", animal.Id);
                    return Unit.Default;
                }

                var existingAnimal = await _repository.GetByIdAsync(animal.Id);
                if (existingAnimal == null)
                {
                    _logger.LogInformation("Animal {} er allerede blevet slettet fra db", animal.Id);
                    return Unit.Default;
                }

                // Slet animal
                await _repository.DeleteAsync(animal.Id);

                // sæt state i statestore til completed
                await _daprClient.SaveStateAsync(
               "statestore",
               stateKey,
               new CompensationState { 
                   IsCompleted = true, 
                   Timestamp = DateTime.UtcNow 
               });

                _logger.LogInformation("Comp. transactions fuldført {}", animal.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Comp. Transaction fejlede {}", animal.Id);
                throw; 
            }

            return Unit.Default;
        }
    }
}


