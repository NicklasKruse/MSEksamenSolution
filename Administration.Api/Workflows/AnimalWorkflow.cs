using Administration.Api.Workflows.Activities;
using Administration.Domain.DomainServices;
using Administration.Domain.Entities;
using CommonAssets.EventDtos;
using Dapr.Workflow;


namespace Administration.Api.Workflows
{
    public class AnimalWorkflow : Workflow<Animal, AnimalCreatedEvent>
    {
        private readonly ISpeciesService SpeciesService;
        private readonly ILogger<AnimalWorkflow> _logger;
        public AnimalWorkflow(ISpeciesService speciesService, ILogger<AnimalWorkflow> logger)
        {
            SpeciesService = speciesService;
            _logger = logger;
        }

        public override async Task<AnimalCreatedEvent> RunAsync(WorkflowContext context, Animal animal)
        {

            var retryOptions = new WorkflowTaskOptions
            {
                // Retry policy - Retry efter 5 sekunder. backOffCoefficient betyder at den fordobler intervallet for hver forsøg. 
                // forhåbningen er at den prøver efter 5, 10, 20 og 40 sekunder
                RetryPolicy = new WorkflowRetryPolicy(firstRetryInterval: TimeSpan.FromSeconds(5), backoffCoefficient: 2.0,
                maxRetryInterval: TimeSpan.FromSeconds(40), maxNumberOfAttempts: 4)
            };

            try
            {
                // Opret Animal vha. CreateAnimalActivity
                var createdAnimal = await context.CallActivityAsync<Animal>(
                    nameof(CreateAnimalActivity),
                    animal,
                    retryOptions);

                var animalCreatedEvent = new AnimalCreatedEvent
                {
                    Id = createdAnimal.Id,
                    Name = createdAnimal.Name,
                };

                // Publish event vha. PublishEventActivity. Bruger AnimalCreatedEvent i stedet for Animal, da det er det vi vil sende.
                await context.CallActivityAsync(
                    nameof(PublishEventActivity),
                    animalCreatedEvent,
                    retryOptions);

                return animalCreatedEvent;
            }
            catch (Exception ex)
            {
                try
                {
                    await context.CallActivityAsync(
                        nameof(CompensateEventPublishActivity),
                        animal,
                        retryOptions);

                    await context.CallActivityAsync(
                        nameof(CompensateAnimalCreationActivity),
                        animal,
                        retryOptions);
                }
                catch (Exception compensationEx)
                {
                    _logger.LogError(compensationEx, "Compensation failed");
                }
                throw;
            }
        }

        #region Placeholder til service registrering
        public AnimalWorkflow()
        {
            //Placeholder til service registrering
        }
        #endregion
    }
}


