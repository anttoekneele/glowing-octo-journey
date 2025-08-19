using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

public class AdminService
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
    {
        _http = factory.CreateClient("Api");
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task AttachAccessTokenAsync()
    {
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
        if (!string.IsNullOrEmpty(accessToken))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    public async Task<bool> CreateCommunicationType(CommunicationTypeDto communicationTypeDto)
    {
        await AttachAccessTokenAsync();
        var response = await _http.PostAsJsonAsync("admin", communicationTypeDto);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Create] Error: {error}");
        }
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateCommunicationType(string typeCode, CommunicationTypeDto communicationTypeDto)
    {
        await AttachAccessTokenAsync();
        var response = await _http.PostAsJsonAsync($"admin/{typeCode}", communicationTypeDto);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Create] Error: {error}");
        }
        return response.IsSuccessStatusCode;
    }
}