using Microsoft.EntityFrameworkCore;

public class Mutation
{
    public async Task<Communication> CreateCommunication([Service] AppDbContext context, Communication input)
    {
        context.Communications.Add(input);
        await context.SaveChangesAsync();
        return input;
    }

    public async Task<bool> UpdateCommunicationStatus([Service] AppDbContext context, Guid id, string newStatus)
    {
        var communication = await context.Communications.FindAsync(id);
        if (communication == null)
        {
            return false;
        }

        communication.CurrentStatus = newStatus;
        communication.LastUpdatedUtc = DateTime.UtcNow;

        context.CommunicationStatusHistories.Add(new CommunicationStatusHistory
        {
            Id = Guid.NewGuid(),
            CommunicationId = communication.Id,
            StatusCode = newStatus,
            OccurredUtc = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<CommunicationType> CreateCommunicationType([Service] AppDbContext context, CommunicationType input)
    {
        context.CommunicationTypes.Add(input);
        await context.SaveChangesAsync();
        return input;
    }

    public async Task<bool> UpdateCommunicationType([Service] AppDbContext context, string typeCode, CommunicationType input)
    {
        var communicationType = await context.CommunicationTypes
            .Include(ct => ct.CommunicationTypeStatuses)
            .FirstOrDefaultAsync(ct => ct.TypeCode == typeCode);

        if (communicationType == null)
        {
            return false;
        }

        communicationType.DisplayName = input.DisplayName;

        context.CommunicationTypeStatuses.RemoveRange(communicationType.CommunicationTypeStatuses);
        communicationType.CommunicationTypeStatuses = input.CommunicationTypeStatuses;

        await context.SaveChangesAsync();
        return true;
    }
}