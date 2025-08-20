using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventPublisher _eventPublisher;

    public EventsController(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    [HttpPost("{eventType}")]
    public async Task<IActionResult> PublishEvent(string eventType, [FromBody] EventPayloadDto payload)
    {
        await _eventPublisher.PublishEventAsync(eventType, payload);
        return Ok();
    }
}
