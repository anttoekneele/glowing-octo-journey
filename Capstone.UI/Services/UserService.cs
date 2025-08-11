using Capstone.Api.Contracts;

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
        return await _http.GetFromJsonAsync<List<CommunicationDto>>("user") ?? [];
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

    public async Task<bool> PublishEvent(string eventName, CommunicationEvent eventData)
    {
        var response = await _http.PostAsJsonAsync($"user/publish/{eventName}", eventData);
        return response.IsSuccessStatusCode;
    }
}