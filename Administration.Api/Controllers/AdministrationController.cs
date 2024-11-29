using Administration.Domain.DomainServices;
using Administration.Domain.Entities;
using Administration.Domain.ValueObjects;
using CommonAssets;
using CommonAssets.EventDtos;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace Administration.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdministrationController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly ISpeciesService SpeciesService;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(DaprClient daprClient, ISpeciesService speciesService, ILogger<AdministrationController> logger)
        {
            _daprClient = daprClient;
            SpeciesService = speciesService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnimal([FromBody] Animal animal)
        {
            try
            {
                var speciesId = new SpeciesId(Guid.NewGuid(), SpeciesService);
                var newAnimal = new Animal(
                    id: Guid.NewGuid(),
                    name: animal.Name,
                    Category: animal.Category,
                    speciesId: speciesId
                );
                newAnimal.SetWeight(new Weight(animal.Weight.Value));

                // Publish
                //var startResponse = await _daprClient.StartWorkflowAsync();
                await _daprClient.PublishEventAsync("pubsub", "animal-created", newAnimal);
                //return Ok();
                return CreatedAtAction(nameof(CreateAnimal), new { id = newAnimal.Id }, newAnimal);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        //Anden version
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Animal animal)
        {
            try
            {
                var speciesId = new SpeciesId(Guid.NewGuid(), SpeciesService);
                var newAnimal = new Animal(
                    id: Guid.NewGuid(),
                    name: animal.Name,
                    Category: animal.Category,
                    speciesId: speciesId
                );
                newAnimal.SetWeight(new Weight(animal.Weight.Value));

                // Create event message
                var eventMessage = new AnimalCreatedEvent
                {
                    Id = newAnimal.Id,
                    Name = newAnimal.Name,
                    //Category = newAnimal.Category,
                    //Weight = newAnimal.Weight.Value,
                    //SpeciesId = newAnimal.SpeciesId.Value
                };

                // Publish with retry logic
                try
                {
                    await _daprClient.PublishEventAsync("pubsub", "animal-created", eventMessage);
                    return CreatedAtAction(nameof(CreateAnimal), new { id = newAnimal.Id }, newAnimal);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to publish animal-created event");
                    // Consider if you want to fail the request or just log the error
                    return StatusCode(500, "Failed to publish event");
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> WorkflowCreateExample(AnimalCreatedEvent animalCreatedEvent)
        {
            await _daprClient.RaiseWorkflowEventAsync("animal-workflow", "create","EventName", animalCreatedEvent);
            return Ok();
        }

        ///////////////////////////////////
        [HttpPost]
        public async Task<IActionResult> NewCreateAnimal([FromBody] Animal animal)
        {
            try
            {
                var workflowId = Guid.NewGuid().ToString();
                var startResponse = await _daprClient.StartWorkflowAsync(
                    "animal-workflow",
                    "dapr",
                    workflowId,
                    animal);

                return AcceptedAtAction(nameof(GetWorkflowStatus),
                    new { workflowId = startResponse.InstanceId },
                    new { workflowId = startResponse.InstanceId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start animal creation workflow");
                return StatusCode(500, new { error = "Failed to process request" });
            }
        }

        [HttpGet("workflow/{workflowId}")]
        public async Task<IActionResult> GetWorkflowStatus(string workflowId)
        {
            try
            {
                var state = await _daprClient.GetWorkflowAsync(workflowId, "dapr");
                return Ok(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get workflow status");
                return StatusCode(500, new { error = "Failed to get workflow status" });
            }
        }

        ///////////////////////////////////
    }
}


