public class CommunicationType
{
    public required string TypeCode { get; set; }
    public required string DisplayName { get; set; }

    public ICollection<Communication>? Communications { get; set; }
    public ICollection<CommunicationTypeStatus>? CommunicationTypeStatuses { get; set; }
}