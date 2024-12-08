using Administration.Api.Workflows;
using Administration.Domain.DomainServices;
using Administration.Domain.Entities;
using Administration.Domain.ValueObjects;
using CommonAssets;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace Administration.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdministrationController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly ISpeciesService _speciesService;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(
            DaprClient daprClient,
            ISpeciesService speciesService,
            ILogger<AdministrationController> logger)
        {
            _daprClient = daprClient;
            _speciesService = speciesService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnimal([FromBody] Animal animal)
        {
            try
            {
                // Id til at tracke workflow
                var workflowId = Guid.NewGuid().ToString();
                _logger.LogInformation($"Starter animal creation workflow: {workflowId}");

                var speciesId = new SpeciesId(Guid.NewGuid(), _speciesService);
                var newAnimal = new Animal(
                    id: Guid.NewGuid(),
                    name: animal.Name,
                    Category: animal.Category,
                    speciesId: speciesId
                );
                newAnimal.SetWeight(new Weight(animal.Weight.Value));


                // Workflow starter
                var startResponse = await _daprClient.StartWorkflowAsync(
                workflowId, // Instance ID
                "dapr", // Workflow component navn. workflow.yaml filen - hedder bare dapr deri.
                nameof(AnimalWorkflow), // Workflow navn
                newAnimal); // Input parameter

                _logger.LogInformation($"{startResponse.InstanceId}");

                return AcceptedAtAction(
                    nameof(GetWorkflowStatus),
                    new { workflowId = startResponse.InstanceId },
                    new
                    {
                        workflowId = startResponse.InstanceId,
                        animalId = newAnimal.Id,
                        status = "Processing"
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create animal");
                return StatusCode(500, new { error = "Failed to process request" });
            }
        }
        /// <summary>
        /// Henter status på workflow. 
        /// https://docs.dapr.io/developing-applications/building-blocks/workflow/howto-manage-workflow/
        /// </summary>
        /// <param name="workflowId"></param>
        /// <returns></returns>
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
                _logger.LogError(ex, $"{workflowId}");
                return StatusCode(500, new { error = "Failed to get workflow status" });
            }
        }
    }

}


