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
    public async Task<ActionResult<CommunicationDto>> CreateCommunication([FromBody] CommunicationDto communicationDto)
    {
        var communication = await _userService.CreateCommunication(communicationDto);
        return CreatedAtAction(nameof(GetCommunicationById), new { id = communication.Id }, communication);
    }

    [HttpGet("communications")]
    public async Task<ActionResult<IEnumerable<CommunicationDto>>> GetAllCommunications()
    {
        var communications = await _userService.GetAllCommunications();
        return Ok(communications);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommunicationDto>> GetCommunicationById(Guid id)
    {
        var communication = await _userService.GetCommunicationById(id);
        if (communication == null)
        {
            return NotFound();
        }
        return Ok(communication);
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateCommunicationStatus(Guid id, [FromBody] CommunicationTypeStatusUpdateDto communicationTypeStatusUpdateDto)
    {
        var updated = await _userService.UpdateCommunicationStatus(id, communicationTypeStatusUpdateDto);
        if (!updated)
        {
            return BadRequest("Invalid status update or communication not found.");
        }
        return NoContent();
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<CommunicationTypeDto>>> GetAllCommunicationTypes()
    {
        var communicationTypes = await _userService.GetAllCommunicationTypes();
        return Ok(communicationTypes);
    }
}