using Administration.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Administration.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdministrationController : ControllerBase
    {
        private readonly DaprClient _daprClient;

        public AnimalController(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnimal(Animal animal)
        {
            // Create animal in administration service
            // Then notify veterinarian service
            await _daprClient.PublishEventAsync("pubsub", "animal-created", animal);
            return Ok();
        }

    }
}
