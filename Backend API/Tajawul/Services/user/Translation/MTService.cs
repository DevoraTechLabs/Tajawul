using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Tajawul.Data.Configuration;
using Tajawul.Interfaces.User.Translation;

namespace Tajawul.Services.user.Translation;

public class MTService(HttpClient httpClient, IOptions<MTOptions> options) : IMTService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiUrl = $"{options.Value.ApiUrl}/t2tt";

    public async Task<string?> GetMTResponseAsync(string text, string tgt_lang, string src_lang)
    {
        var payload = new { text, tgt_lang, src_lang };
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_apiUrl, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to call API. Status code: {response.StatusCode}");
        }

        var responseData = await response.Content.ReadAsStringAsync();

        // Use JsonDocument to parse without a model
        using var doc = JsonDocument.Parse(responseData);

        // Navigate the JSON structure manually
        if (doc.RootElement.TryGetProperty("translated_text", out var translatedTextElement))
        {
            return translatedTextElement.GetString();
        }

        return null;
    }
}