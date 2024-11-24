using Microsoft.AspNetCore.Mvc;

namespace Veterinarian.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VeterinarianController : ControllerBase
    {
        [Topic("pubsub", "animal-created")]
        [HttpPost("animal-created")]
        public async Task<IActionResult> HandleAnimalCreated(Animal animal)
        {
            // Handle new animal creation
            return Ok();
        }
    }
}
