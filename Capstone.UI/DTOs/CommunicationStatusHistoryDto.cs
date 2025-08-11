public class CommunicationStatusHistoryDto
{
    public Guid Id { get; set; }
    public Guid CommunicationId { get; set; }
    public required string StatusCode { get; set; }
    public DateTime OccurredUtc { get; set; }

    public CommunicationDto? Communication { get; set; }
}