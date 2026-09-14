using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Tajawul.Data.Configuration;
using Tajawul.Interfaces.User.ChatBot;

namespace Tajawul.Services.User.Chatbot
{
    public class LlmService : ILlmService
    {
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;

        public LlmService(HttpClient httpClient, IOptions<LLMOptions> options)
        {
            _httpClient = httpClient;
            _apiBaseUrl = options.Value.ApiUrl;
        }

        public async Task<string?> GetLlmResponseAsync(string prompt, string token)
        {
            var requestUrl = $"{_apiBaseUrl}/chat/"; // No need to include token or prompt in URL

            var requestBody = new
            {
                user_token = token,
                user_input = prompt
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync(requestUrl, requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to get response from chatbot. Status: {response.StatusCode}, Response: {content}");
                }

                var result = await response.Content.ReadFromJsonAsync<ChatResponse>();

                return result?.Response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error calling Python chatbot API.", ex);
            }
        }

        //public async Task<string?> GetLlmResponseAsync(string prompt, string token)
        //{
        //    var requestUrl = $"{_apiBaseUrl}/chat/{Uri.EscapeDataString(token)}/{Uri.EscapeDataString(prompt)}";

        //    try
        //    {
        //        var response = await _httpClient.GetAsync(requestUrl);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            throw new Exception($"Failed to get response from chatbot. Status: {response.StatusCode}, Response: {content}");
        //        }

        //        var result = await response.Content.ReadFromJsonAsync<ChatResponse>();

        //        return result?.Response;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error calling Python chatbot API.", ex);
        //    }
        //}
    }

    public class ChatResponse
    {
        [JsonPropertyName("response")]
        public string? Response { get; set; }
    }
}