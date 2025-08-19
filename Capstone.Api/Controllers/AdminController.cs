using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CommunicationTypeDto>> CreateCommunicationType([FromBody] CommunicationTypeDto communicationTypeDto)
    {
        if (communicationTypeDto.TypeCode == "" || !communicationTypeDto.CommunicationTypeStatuses.Any())
        {
            return BadRequest("Invalid communication type data.");
        }
        var communicationType = await _adminService.CreateCommunicationType(communicationTypeDto);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{typeCode}")]
    public async Task<IActionResult> UpdateCommunicationType(string typeCode, [FromBody] CommunicationTypeDto communicationTypeDto)
    {
        if (communicationTypeDto.TypeCode == "" || !communicationTypeDto.CommunicationTypeStatuses.Any())
        {
            return BadRequest("Invalid communication type data.");
        }
        var updated = await _adminService.UpdateCommunicationType(typeCode, communicationTypeDto);
        if (!updated)
        {
            return BadRequest("Invalid type update or type not found.");
        }
        return NoContent();
    }
}
