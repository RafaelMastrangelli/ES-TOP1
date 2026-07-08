using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ESTop1.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ESTop1.Infrastructure.Services;

/// <summary>
/// Cliente HTTP para APIs compatíveis com OpenAI Chat Completions (OpenAI, Groq, etc.).
/// </summary>
public class OpenAiCompatibleChatService : ILLMChatService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenAiCompatibleChatService> _logger;

    public OpenAiCompatibleChatService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<OpenAiCompatibleChatService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> CompleteAsync(
        string systemPrompt,
        string userPrompt,
        int maxTokens,
        CancellationToken cancellationToken = default)
    {
        var apiKey = ObterApiKey();
        var model = _configuration["LLM:Model"] ?? "gpt-3.5-turbo";

        var request = new
        {
            model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            max_tokens = maxTokens,
            temperature = 0.1
        };

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
        {
            Content = JsonContent.Create(request)
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("LLM retornou {StatusCode}: {Body}", (int)response.StatusCode, body);
            throw new InvalidOperationException($"Erro na API LLM ({(int)response.StatusCode})");
        }

        using var document = JsonDocument.Parse(body);
        var content = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return content?.Trim() ?? string.Empty;
    }

    private string ObterApiKey()
    {
        var provider = _configuration["LLM:Provider"] ?? "OpenAI";

        var apiKey = provider.Equals("Groq", StringComparison.OrdinalIgnoreCase)
            ? _configuration["LLM:GroqApiKey"] ?? Environment.GetEnvironmentVariable("GROQ_API_KEY")
            : _configuration["LLM:ApiKey"]
              ?? _configuration["OpenAI:ApiKey"]
              ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("API key ausente para provedor LLM {Provider}", provider);
            throw new InvalidOperationException($"API key não configurada para o provedor LLM '{provider}'");
        }

        return apiKey;
    }
}
