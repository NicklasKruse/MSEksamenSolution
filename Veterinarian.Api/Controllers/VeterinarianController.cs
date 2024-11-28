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
        private readonly ILogger<VeterinarianController> _logger;
        public VeterinarianController(ILogger<VeterinarianController> logger)
        {
            _logger = logger;
        }
        //Repo interface?
        //Logger?
        [Topic("pubsub", "animal-created")]
        [HttpPost("animal-created")]
        public async Task<IActionResult> HandleAnimalCreated(CaseId Case)
        {
            var caseId = new CaseId(Case.Value);
            var appointment = new Appointment(caseId);
            //Yderligere oprettelse af appointment, til persistens (når jeg har en repo implementering)
            return Ok(new
            {
                AppointmentId = appointment.Id,
                CaseId = caseId.Value
            });
        }

        // anden version
        [Topic("pubsub", "animal-created")]
        [HttpPost("animal-created")]
        public async Task<IActionResult> AnimalCreated(AnimalCreatedEvent animalEvent)
        {
            try
            {
                _logger.LogInformation("Received animal-created event for animal: {AnimalId}", animalEvent.Id);

                var caseId = new CaseId(Guid.NewGuid());
                var appointment = new Appointment(caseId)
                {
                    Id = animalEvent.Id,
                    CreatedAtTime = DateTime.UtcNow
                };

                //await _appointmentRepository.SaveAppointmentAsync(appointment); Repo

                return Ok(new
                {
                    AppointmentId = appointment.Id,
                    CaseId = caseId.Value
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing animal-created event");
                return StatusCode(500, new { error = "Failed to process animal creation" });
            }
        }
    }
}
