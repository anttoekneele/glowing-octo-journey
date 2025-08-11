using Microsoft.AspNetCore.Mvc;
using Capstone.Api.Contracts;
using Capstone.Api.Services;

namespace Capstone.Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly RabbitMqPublisher _publisher;

        public EventController(RabbitMqPublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpPost("{eventName}")]
        public IActionResult PublishEvent(string eventName, [FromBody] CommunicationEvent communicationEvent)
        {
            _publisher.Publish(eventName, communicationEvent);
            return Ok();
        }
    }
}
