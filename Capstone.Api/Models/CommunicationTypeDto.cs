public class CommunicationTypeDto
{
    public required string TypeCode { get; set; }
    public required string DisplayName { get; set; }

    public ICollection<CommunicationDto>? Communications { get; set; }
    public ICollection<CommunicationTypeStatusDto>? CommunicationTypeStatuses { get; set; }
}