using Microsoft.EntityFrameworkCore;
using Capstone.Api.Contracts;
using System.Net.Http;
using System.Net.Http.Json;

public class UserService : IUserService
{
    private readonly AppDbContext _appDbContext;
    private readonly HttpClient _http;


    public UserService(AppDbContext appDbContext, HttpClient http)
    {
        _appDbContext = appDbContext;
        _http = http;
    }

    public async Task<CommunicationDto> CreateCommunication(CommunicationDto communicationDto)
    {
        var typeCodeExist = await _appDbContext.CommunicationTypes
            .AnyAsync(communicationType => communicationType.TypeCode == communicationDto.TypeCode);

        if (!typeCodeExist)
        {
            throw new ArgumentException("Invalid TypeCode.");
        }

        var validStatusCode = await _appDbContext.CommunicationTypeStatuses
            .AnyAsync(communicationTypeStatus => communicationTypeStatus.TypeCode == communicationDto.TypeCode && communicationTypeStatus.StatusCode == communicationDto.CurrentStatus);

        if (!validStatusCode)
            throw new ArgumentException("Invalid initial status for this communication type.");

        var communication = new Communication
        {
            Id = Guid.NewGuid(),
            Title = communicationDto.Title,
            TypeCode = communicationDto.TypeCode,
            CurrentStatus = communicationDto.CurrentStatus,
            LastUpdatedUtc = DateTime.UtcNow
        };

        _appDbContext.Communications.Add(communication);

        var communicationStatusHistory = new CommunicationStatusHistory
        {
            Id = Guid.NewGuid(),
            CommunicationId = communication.Id,
            StatusCode = communication.CurrentStatus,
            OccurredUtc = communication.LastUpdatedUtc
        };

        _appDbContext.CommunicationStatusHistories.Add(communicationStatusHistory);

        await _appDbContext.SaveChangesAsync();

        return MapToDto(communication);
    }

    public async Task<IEnumerable<CommunicationDto>> GetAllCommunications()
    {
        var communications = await _appDbContext.Communications
            .Include(communication => communication.CommunicationType)
            .Include(communication => communication.CommunicationStatusHistories)
            .ToListAsync();

        return communications.Select(MapToDto).ToList();
    }

    public async Task<CommunicationDto?> GetCommunicationById(Guid id)
    {
        var communication = await _appDbContext.Communications
            .Include(communication => communication.CommunicationType)
            .Include(communication => communication.CommunicationStatusHistories)
            .FirstOrDefaultAsync(communication => communication.Id == id);

        if (communication == null)
        {
            return null;
        }

        return MapToDto(communication);
    }

    public async Task<bool> UpdateCommunicationStatus(Guid id, CommunicationTypeStatusUpdateDto communicationTypeStatusUpdateDto)
    {
        var communication = await _appDbContext.Communications.FindAsync(id);

        if (communication == null)
        {
            return false;
        }

        var validStatusCode = await _appDbContext.CommunicationTypeStatuses
            .AnyAsync(communicationTypeStatus => communicationTypeStatus.TypeCode == communication.TypeCode && communicationTypeStatus.StatusCode == communicationTypeStatusUpdateDto.StatusCode);

        if (!validStatusCode)
        {
            throw new ArgumentException("Invalid status for this communication type.");
        }

        communication.CurrentStatus = communicationTypeStatusUpdateDto.StatusCode;

        communication.LastUpdatedUtc = DateTime.UtcNow;

        var communicationStatusHistory = new CommunicationStatusHistory
        {
            Id = Guid.NewGuid(),
            CommunicationId = communication.Id,
            StatusCode = communicationTypeStatusUpdateDto.StatusCode,
            OccurredUtc = communication.LastUpdatedUtc
        };

        _appDbContext.CommunicationStatusHistories.Add(communicationStatusHistory);

        await _appDbContext.SaveChangesAsync();

        return true;
    }

    private CommunicationDto MapToDto(Communication communication)
    {
        var response = new CommunicationDto
        {
            Id = communication.Id,
            Title = communication.Title,
            TypeCode = communication.TypeCode,
            CurrentStatus = communication.CurrentStatus,
            LastUpdatedUtc = communication.LastUpdatedUtc,
            CommunicationType = communication.CommunicationType is null ? null : new CommunicationTypeDto
            {
                TypeCode = communication.CommunicationType.TypeCode,
                DisplayName = communication.CommunicationType.DisplayName
            },
            CommunicationStatusHistories = communication.CommunicationStatusHistories?.Select(communicationStatusHistory => new CommunicationStatusHistoryDto
            {
                StatusCode = communicationStatusHistory.StatusCode,
                OccurredUtc = communicationStatusHistory.OccurredUtc
            }).ToList()
        };

        return response;
    }

    public async Task<bool> PublishEvent(string eventName, CommunicationEvent communicationEvent)
    {
        var response = await _http.PostAsJsonAsync($"events/{eventName}", communicationEvent);
        return response.IsSuccessStatusCode;
    }
}
