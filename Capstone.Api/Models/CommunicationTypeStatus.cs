public class CommunicationTypeStatus
{
    public required string TypeCode { get; set; }
    public required string StatusCode { get; set; }
    public required string Description { get; set; }

    public CommunicationType? CommunicationType { get; set; }
}