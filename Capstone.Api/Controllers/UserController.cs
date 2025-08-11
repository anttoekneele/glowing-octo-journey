using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<CommunicationDto>> Create([FromBody] CommunicationDto communicationDto)
    {
        var communication = await _userService.CreateCommunication(communicationDto);

        return CreatedAtAction(nameof(Get), new { id = communication.Id }, communication);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommunicationDto>>> GetAll()
    {
        var communications = await _userService.GetAllCommunications();
        return Ok(communications);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommunicationDto>> Get(Guid id)
    {
        var communication = await _userService.GetCommunicationById(id);
        if (communication == null)
        {
            return NotFound();
        }
        return Ok(communication);
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] CommunicationTypeStatusUpdateDto communicationTypeStatusUpdateDto)
    {
        var updated = await _userService.UpdateCommunicationStatus(id, communicationTypeStatusUpdateDto);
        if (!updated)
        {
            return BadRequest("Invalid status update or communication not found.");
        }
        return NoContent();
    }
}