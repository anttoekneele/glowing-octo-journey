public class UserService
{
    private readonly HttpClient _http;

    public UserService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("Api");
    }

    public async Task<bool> CreateCommunication(CommunicationDto communicationDto)
    {
        var response = await _http.PostAsJsonAsync("user", communicationDto);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<CommunicationDto>> GetAllCommunications()
    {
        return await _http.GetFromJsonAsync<List<CommunicationDto>>("user/communications") ?? [];
    }

    public async Task<CommunicationDto?> GetCommunicationById(Guid id)
    {
        return await _http.GetFromJsonAsync<CommunicationDto>($"user/{id}");
    }

    public async Task<bool> UpdateCommunicationStatus(Guid id, CommunicationTypeStatusUpdateDto communicationTypeStatusUpdateDto)
    {
        var response = await _http.PostAsJsonAsync($"user/{id}/status", communicationTypeStatusUpdateDto);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<CommunicationTypeDto>> GetAllCommunicationTypes()
    {
        return await _http.GetFromJsonAsync<List<CommunicationTypeDto>>("user/types") ?? [];
    }
}