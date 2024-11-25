using CommonAssets.EventDtos;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Veterinarian.Domain;
using Veterinarian.Domain.ValueObjects;

namespace Veterinarian.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VeterinarianController : ControllerBase
    {
        //Repo interface?
        //Logger?
        [Topic("pubsub", "animal-created")]
        [HttpPost("animal-created")]
        public async Task<IActionResult> HandleAnimalCreated([FromBody] AnimalCreatedEvent eventData)
        {
            var caseId = new CaseId(eventData.Id);
            var appointment = new Appointment(caseId);
            //Yderligere oprettelse af appointment, til persistens (når jeg har en repo implementering)
            return Ok(new
            {
                AppointmentId = appointment.Id,
                CaseId = caseId.Value
            });
        }
    }
}
