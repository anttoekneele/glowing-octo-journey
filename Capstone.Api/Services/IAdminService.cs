public interface IAdminService
{
    Task<CommunicationTypeDto> CreateCommunicationType(CommunicationTypeDto communicationTypeDto);
    Task<bool> UpdateCommunicationType(string typeCode, CommunicationTypeDto communicationTypeDto);
}