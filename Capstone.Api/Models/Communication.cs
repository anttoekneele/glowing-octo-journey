public class Communication
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string TypeCode { get; set; }
    public required string CurrentStatus { get; set; }
    public DateTime LastUpdatedUtc { get; set; }

    public CommunicationType? CommunicationType { get; set; }
    public ICollection<CommunicationStatusHistory>? CommunicationStatusHistories { get; set; }
}