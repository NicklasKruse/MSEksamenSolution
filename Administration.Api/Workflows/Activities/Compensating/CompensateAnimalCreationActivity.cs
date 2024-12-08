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
        private readonly IRepositoryEx _repository; //repo eksempel

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

                var existingAnimal = await _repository.GetByIdAsync(animal.Id);
                if (existingAnimal == null)
                {
                    _logger.LogInformation("Animal {} already deleted, skipping compensation", animal.Id);
                    return Unit.Default;
                }

                // vi bruger en statestore til at holde styr på om vi har fuldført kompensationen
                var compensationKey = $"compensation-tracking-{animal.Id}"; //giv den et unikt navn vha. animal.Id. 
                var compensationState = await _daprClient.GetStateAsync<CompensationState>(
                    "statestore",
                    compensationKey); 

                // her tjekker vi om kompensationen allerede er fuldført
                if (compensationState?.IsCompleted == true)
                {
                    _logger.LogInformation("Compensation allerede fuldført {}", animal.Id);
                    return Unit.Default;
                }

                // Slet animal
                await _repository.DeleteAsync(animal.Id);

                // Event til pubsub om at animal er slettet
                await _daprClient.PublishEventAsync(
                    "pubsub",
                    "animal-deleted",
                    new { AnimalId = animal.Id, Timestamp = DateTime.UtcNow });

                // sæt state i statestore til completed
                await _daprClient.SaveStateAsync(
               "statestore",
               compensationKey,
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
    /// <summary>
    /// Repo eksempel
    /// </summary>
    public interface IRepositoryEx
    {
        Task<Animal> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);

        Task<Animal> CreateAsync(Animal animal);// til CreateAnimalActivity
    }
}


