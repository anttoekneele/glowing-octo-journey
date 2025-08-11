public class CommunicationDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string TypeCode { get; set; }
    public required string CurrentStatus { get; set; }
    public DateTime LastUpdatedUtc { get; set; }

    public CommunicationTypeDto? CommunicationType { get; set; }
    public ICollection<CommunicationStatusHistoryDto>? CommunicationStatusHistories { get; set; }
}