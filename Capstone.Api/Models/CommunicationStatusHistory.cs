public class CommunicationStatusHistory
{
    public Guid Id { get; set; }
    public Guid CommunicationId { get; set; }
    public required string StatusCode { get; set; }
    public DateTime OccurredUtc { get; set; }

    public Communication? Communication { get; set; }
}