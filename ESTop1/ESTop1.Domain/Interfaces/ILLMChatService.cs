namespace ESTop1.Domain.Interfaces;

/// <summary>
/// Contrato genérico para provedores compatíveis com OpenAI Chat Completions (OpenAI, Groq, etc.).
/// </summary>
public interface ILLMChatService
{
    Task<string> CompleteAsync(
        string systemPrompt,
        string userPrompt,
        int maxTokens,
        CancellationToken cancellationToken = default);
}
