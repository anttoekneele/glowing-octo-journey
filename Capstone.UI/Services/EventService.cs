public class EventService
{
    private readonly HttpClient _http;

    public EventService(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("Api");
    }

    public async Task PublishAsync(string eventType, EventPayloadDto payload)
    {
        var response = await _http.PostAsJsonAsync($"events/{eventType}", payload);
        response.EnsureSuccessStatusCode();
    }
}
