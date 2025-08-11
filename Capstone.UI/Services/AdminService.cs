public class AdminService
{
    private readonly HttpClient _http;

    public AdminService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("Api");
    }

    public async Task<List<CommunicationTypeDto>> GetAllCommunicationTypes()
    {
        return await _http.GetFromJsonAsync<List<CommunicationTypeDto>>("admin") ?? [];
    }

    public async Task<bool> SaveCommunicationType(CommunicationTypeDto communicationTypeDto)
    {
        var response = await _http.PostAsJsonAsync("admin", communicationTypeDto);
        return response.IsSuccessStatusCode;
    }
}