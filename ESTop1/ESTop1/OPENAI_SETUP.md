# Configuração do provedor LLM (OpenAI / Groq)

A busca inteligente usa a interface `ILLMChatService`, compatível com APIs no formato OpenAI Chat Completions.

## OpenAI (padrão)

```json
"LLM": {
  "Provider": "OpenAI",
  "ApiKey": "sk-sua-chave",
  "BaseUrl": "https://api.openai.com/v1",
  "Model": "gpt-3.5-turbo"
}
```

Ou via user secrets:

```bash
dotnet user-secrets set "LLM:ApiKey" "sk-sua-chave-real-aqui"
```

## Groq (migração futura)

Para trocar para Groq, altere apenas a configuração — **sem mudar código**:

```json
"LLM": {
  "Provider": "Groq",
  "GroqApiKey": "gsk_sua-chave",
  "Model": "llama-3.1-8b-instant"
}
```

Variável de ambiente alternativa: `GROQ_API_KEY`

Modelos sugeridos na Groq:
- `llama-3.1-8b-instant` — rápido e econômico
- `llama-3.3-70b-versatile` — mais preciso

## Verificar

```bash
cd ESTop1/ESTop1
dotnet run
```

Teste no frontend: página Jogadores → busca com IA.
