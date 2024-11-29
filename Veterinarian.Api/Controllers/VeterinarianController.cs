using CommonAssets.EventDtos;
using Dapr;
using Dapr.Client;
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
        private readonly DaprClient _daprClient;
        //private readonly IAppointmentRepository _appointmentRepository;
        public VeterinarianController(ILogger<VeterinarianController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
            //_appointmentRepository = appointmentRepository;
        }

        [Topic("pubsub", "animal-created")]
        [HttpPost("animal-created")]
        public async Task<IActionResult> HandleAnimalCreated(AnimalCreatedEvent animalEvent)
        {
            _logger.LogInformation($"Her bekræfter vi at vi har modtaget animal-created event:", animalEvent.Id);
            try
            {
                var caseId = new CaseId(Guid.NewGuid()); //Ideen er at det her skal være selve animal objekted
                var appointment = new Appointment(caseId) // Og det her skal være den interne appointment, Hvori der er en reference til det specfikke animal
                {
                    Id = animalEvent.Id,
                    CreatedAtTime = DateTime.UtcNow
                };

                //Yderligere oprettelse af appointment, til persistens (når jeg har en repo implementering)
                //await _appointmentRepository.SaveAppointmentAsync(appointment);

                await _daprClient.PublishEventAsync("pubsub", "appointment-created", new
                {
                    AppointmentId = appointment.Id,
                    CaseId = caseId.Value,
                    Status = "Success"
                });

                return Ok(new
                {
                    AppointmentId = appointment.Id,
                    CaseId = caseId.Value
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing animal-created event");

                // Publish failure event for saga compensation
                await _daprClient.PublishEventAsync("pubsub", "appointment-failed", new
                {
                    AnimalId = animalEvent.Id,
                    Error = ex.Message
                });

                return StatusCode(500, new { error = "Failed to process animal creation" });
            }
        }

        // Cancelled version
        [Topic("pubsub", "animal-creation-cancelled")]
        [HttpPost("animal-creation-cancelled")]
        public async Task<IActionResult> HandleAnimalCreationCancelled(AnimalCreationCancelledEvent cancelEvent)
        {
            try
            {
                // Compensating transaction - cancel appointment
                //await _appointmentRepository.CancelAppointmentAsync(cancelEvent.AnimalId);

                _logger.LogInformation("Bekræfter at vi har modtaget animal-created event", cancelEvent.AnimalId);

                //animal-creation-cancelled / appointment-cancelled ??
                await _daprClient.PublishEventAsync("pubsub", "appointment-cancelled", new
                {
                    AnimalId = cancelEvent.AnimalId,
                    Status = "Cancelled"
                });

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fejlbesked i tilfælde af at flow der skal aflyse en appointment ikke virker");
                return StatusCode(500, new { error = "fejlbesked angående fejlede flow" });
            }
        }
    }
}
