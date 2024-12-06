using Administration.Api.Workflows.Activities;
using Administration.Domain.DomainServices;
using Administration.Domain.Entities;
using Administration.Domain.ValueObjects;
using CommonAssets;
using CommonAssets.EventDtos;
using Dapr.Workflow;


namespace Administration.Api.Workflows
{
    public partial class AnimalWorkflow : Workflow<Animal, AnimalCreatedEvent>
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

            // Denne activity er uden for try/catch scope, udelukkende for at kunne bruge newAnimal i min catch block til at starte en compensating transaction
            // Så må vi bare håbe denne activity ikke fejler, for så er der ingen compensating transaction :D skal nok lige tænke over det
            var speciesId = new SpeciesId(Guid.NewGuid(), SpeciesService);

            var newAnimal = await context.CallActivityAsync<Animal>(
                nameof(CreateAnimalActivity),
                new CreateAnimalRequest(animal, speciesId));

            newAnimal.SetWeight(new Weight(animal.Weight.Value));

            try
            {
                var animalCreatedEvent = new AnimalCreatedEvent
                {
                    Id = newAnimal.Id,
                    Name = newAnimal.Name,
                };

                // Publish 
                await context.CallActivityAsync(
                    nameof(PublishEventActivity),
                    new PublishEventRequest("pubsub", "animal-created", animalCreatedEvent), retryOptions);

                //await context.WaitForExternalEventAsync<>(); external event eksempel

                // notification
                await context.CallActivityAsync(
                    nameof(NotifyActivity),
                    new Notification("Activity called", newAnimal));

                return animalCreatedEvent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Workflow failed, executing compensation");

                if (newAnimal != null)
                {
                    try
                    {
                        // Compensate i omvendt rækkefølge
                        await context.CallActivityAsync(
                            nameof(CompensateNotificationActivity),
                            newAnimal,
                            retryOptions);

                        await context.CallActivityAsync(
                            nameof(CompensateEventPublishActivity),
                            newAnimal,
                            retryOptions);

                        await context.CallActivityAsync(
                            nameof(CompensateAnimalCreationActivity),
                            newAnimal,
                            retryOptions);

                        
                    }
                    catch (Exception compensationEx)
                    {
                        _logger.LogError(compensationEx, "Compensation failed");
                        // Hvis vores compensating transactions fejler
                    }
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


