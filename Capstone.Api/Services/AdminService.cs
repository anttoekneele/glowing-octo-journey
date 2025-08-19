using Microsoft.EntityFrameworkCore;

public class AdminService : IAdminService
{
    private readonly AppDbContext _appDbContext;

    public AdminService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task<CommunicationTypeDto> CreateCommunicationType(CommunicationTypeDto communicationTypeDto)
    {
        var communicationType = new CommunicationType
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
        _appDbContext.CommunicationTypes.Add(communicationType);

        await _appDbContext.SaveChangesAsync();
        return MapToCommunicationTypeDto(communicationType);
    }

    public async Task<bool> UpdateCommunicationType(string typeCode, CommunicationTypeDto communicationTypeDto)
    {
        var communicationType = await _appDbContext.CommunicationTypes
            .Include(communicationType => communicationType.CommunicationTypeStatuses)
            .FirstOrDefaultAsync(communicationType => communicationType.TypeCode == typeCode);
        if (communicationType == null)
        {
            return false;
        }

        communicationType.DisplayName = communicationTypeDto.DisplayName;
        _appDbContext.CommunicationTypeStatuses.RemoveRange(communicationType.CommunicationTypeStatuses);
        communicationType.CommunicationTypeStatuses = communicationTypeDto.CommunicationTypeStatuses.Select(communicationTypeStatus => new CommunicationTypeStatus
        {
            TypeCode = communicationTypeDto.TypeCode,
            StatusCode = communicationTypeStatus.StatusCode,
            Description = communicationTypeStatus.Description
        }).ToList();

        await _appDbContext.SaveChangesAsync();
        return true;
    }

    private CommunicationTypeDto MapToCommunicationTypeDto(CommunicationType communicationType)
    {
        var response = new CommunicationTypeDto
        {
            TypeCode = communicationType.TypeCode,
            DisplayName = communicationType.DisplayName,
            CommunicationTypeStatuses = communicationType.CommunicationTypeStatuses?.Select(communicationTypeStatus => new CommunicationTypeStatusDto
            {
                TypeCode = communicationTypeStatus.TypeCode,
                StatusCode = communicationTypeStatus.StatusCode,
                Description = communicationTypeStatus.Description
            }).ToList()
        };
        return response;
    }
}