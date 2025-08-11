public interface IUserService
{
    Task<CommunicationDto> CreateCommunication(CommunicationDto communicationDto);
    Task<IEnumerable<CommunicationDto>> GetAllCommunications();
    Task<CommunicationDto?> GetCommunicationById(Guid id);
    Task<bool> UpdateCommunicationStatus(Guid id, CommunicationTypeStatusUpdateDto communicationTypeStatusUpdateDto);
}