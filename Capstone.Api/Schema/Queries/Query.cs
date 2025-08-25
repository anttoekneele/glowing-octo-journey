using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

public class Query
{
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Communication> GetCommunications([Service] AppDbContext context)
        => context.Communications;

    public async Task<Communication?> GetCommunicationById([Service] AppDbContext context, Guid id)
        => await context.Communications
            .Include(c => c.CommunicationType)
            .Include(c => c.CommunicationStatusHistories)
            .FirstOrDefaultAsync(c => c.Id == id);

    public IQueryable<CommunicationType> GetCommunicationTypes([Service] AppDbContext context)
        => context.CommunicationTypes;

    // [Authorize]
    // public string Health() => "Healthy";
}