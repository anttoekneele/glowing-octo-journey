using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _appDbContext;
    public AdminController(AppDbContext appDbContext) => _appDbContext = appDbContext;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommunicationTypeDto>>> GetTypes()
    {
        var communicationTypes = await _appDbContext.CommunicationTypes
            .Include(communicationType => communicationType.CommunicationTypeStatuses)
            .ToListAsync();

        var response = communicationTypes.Select(communicationType => new CommunicationTypeDto
        {
            TypeCode = communicationType.TypeCode,
            DisplayName = communicationType.DisplayName,
            CommunicationTypeStatuses = communicationType.CommunicationTypeStatuses?.Select(communicationTypeStatus => new CommunicationTypeStatusDto
            {
                TypeCode = communicationTypeStatus.TypeCode,
                StatusCode = communicationTypeStatus.StatusCode,
                Description = communicationTypeStatus.Description
            }).ToList()
        });

        return Ok(response);
    }

    // [HttpGet]
    // public async Task<ActionResult<IEnumerable<CommunicationTypeStatusDto>>> GetStatuses()
    // {
    //     var communicationTypeStatuses = await _appDbContext.CommunicationTypeStatuses
    //     .Include(communicationTypeStatus => communicationTypeStatus.CommunicationType)
    //     .ToListAsync();

    //     var response = communicationTypeStatuses.Select(communicationTypeStatus => new CommunicationTypeStatusDto
    //     {

    //     });
    // }

    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateType([FromBody] CommunicationTypeDto communicationTypeDto)
    {
        if (communicationTypeDto.TypeCode is null || communicationTypeDto.CommunicationTypeStatuses is null)
        {
            return BadRequest("Invalid communication type data.");
        }
        var communicationType = await _appDbContext.CommunicationTypes
            .Include(communicationType => communicationType.CommunicationTypeStatuses)
            .FirstOrDefaultAsync(communicationType => communicationType.TypeCode == communicationTypeDto.TypeCode);

        if (communicationType!= null)
        {
            communicationType.DisplayName = communicationTypeDto.DisplayName;

            if (communicationType.CommunicationTypeStatuses != null && communicationType.CommunicationTypeStatuses.Any())
            {
                _appDbContext.CommunicationTypeStatuses.RemoveRange(communicationType.CommunicationTypeStatuses);
            }

            communicationType.CommunicationTypeStatuses = communicationTypeDto.CommunicationTypeStatuses.Select(communicationTypeStatus => new CommunicationTypeStatus
            {
                TypeCode = communicationTypeDto.TypeCode,
                StatusCode = communicationTypeStatus.StatusCode,
                Description = communicationTypeStatus.Description
            }).ToList();
        }
        else
        {
            var newCommunicationType = new CommunicationType
            {
                TypeCode = communicationTypeDto.TypeCode,
                DisplayName = communicationTypeDto.DisplayName,
                CommunicationTypeStatuses = communicationTypeDto.CommunicationTypeStatuses.Select(communicationTypeStatus => new CommunicationTypeStatus
                {
                    TypeCode = communicationTypeDto.TypeCode,
                    StatusCode = communicationTypeStatus.StatusCode,
                    Description = communicationTypeStatus.Description
                }).ToList()
            };

            _appDbContext.CommunicationTypes.Add(newCommunicationType);
        }

        await _appDbContext.SaveChangesAsync();

        return Ok();
    }
}
