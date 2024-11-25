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
        private readonly ISpeciesService SpeciesService;

        public AdministrationController(DaprClient daprClient, ISpeciesService speciesService)
        {
            _daprClient = daprClient;
            SpeciesService = speciesService;
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

    }
}


