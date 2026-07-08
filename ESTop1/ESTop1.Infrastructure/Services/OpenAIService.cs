using Azure.AI.OpenAI;
using ESTop1.Domain;
using ESTop1.Domain.DTOs;
using ESTop1.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ESTop1.Infrastructure.Services;

public class OpenAIService : IOpenAIService
{
    private const string DefaultFotoUrl = "/player-default.jpg";
    private readonly IJogadorRepository _jogadorRepository;
    private readonly ITimeRepository _timeRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(
        IJogadorRepository jogadorRepository,
        ITimeRepository timeRepository,
        IConfiguration configuration,
        ILogger<OpenAIService> logger)
    {
        _jogadorRepository = jogadorRepository;
        _timeRepository = timeRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<(int StatusCode, object Payload)> BuscarJogadoresAsync(string consulta, CancellationToken cancellationToken = default)
    {
        var jogadorExistente = await _jogadorRepository.ObterPorApelidoAsync(consulta, cancellationToken);

        if (jogadorExistente is not null)
        {
            return (200, CriarPayloadJogadorExistente(jogadorExistente, consulta));
        }

        var dadosIA = await BuscarDadosJogadorPorCampos(consulta, cancellationToken);

        var jogadorConcorrente = await _jogadorRepository.ObterPorApelidoAsync(consulta, cancellationToken);

        if (jogadorConcorrente is not null)
        {
            return (200, CriarPayloadJogadorConcorrente(jogadorConcorrente, consulta));
        }

        var novoJogador = new Jogador
        {
            Id = Guid.NewGuid(),
            Apelido = dadosIA.Apelido,
            Pais = dadosIA.Pais,
            Idade = dadosIA.Idade,
            FuncaoPrincipal = ParseOuPadrao(dadosIA.FuncaoPrincipal, Funcao.Entry),
            Status = ParseOuPadrao(dadosIA.Status, StatusJogador.Profissional),
            Disponibilidade = string.IsNullOrEmpty(dadosIA.TimeAtual) ? Disponibilidade.Livre : Disponibilidade.EmTime,
            ValorDeMercado = dadosIA.ValorDeMercado,
            FotoUrl = DefaultFotoUrl,
            Visivel = true
        };

        novoJogador.Estatisticas.Add(CriarEstatisticaInicial(novoJogador.Id, dadosIA.Rating));
        await AssociarTimeAsync(novoJogador, dadosIA.TimeAtual, dadosIA.Pais, cancellationToken);

        var estatistica = novoJogador.Estatisticas.First();
        var jogadorCriado = await _jogadorRepository.CriarAsync(novoJogador, cancellationToken);

        return (201, CriarPayloadJogadorCriado(jogadorCriado, estatistica, dadosIA, consulta));
    }

    public async Task<(int StatusCode, object Payload)> BuscarJogadoresTesteAsync(string consulta, CancellationToken cancellationToken = default)
    {
        var jogadorExistente = await _jogadorRepository.ObterPorApelidoAsync(consulta, cancellationToken);

        if (jogadorExistente is not null)
        {
            return (200, CriarPayloadJogadorExistente(jogadorExistente, consulta));
        }

        var dadosIA = await BuscarDadosJogadorPorCampos(consulta, cancellationToken);

        var jogadorConcorrente = await _jogadorRepository.ObterPorApelidoAsync(consulta, cancellationToken);

        if (jogadorConcorrente is not null)
        {
            return (200, CriarPayloadJogadorConcorrente(jogadorConcorrente, consulta));
        }

        var novoJogador = new Jogador
        {
            Id = Guid.NewGuid(),
            Apelido = consulta.Trim(),
            Pais = dadosIA.Pais,
            Idade = dadosIA.Idade,
            FuncaoPrincipal = ParseOuPadrao(dadosIA.FuncaoPrincipal, Funcao.Entry),
            Status = ParseOuPadrao(dadosIA.Status, StatusJogador.Profissional),
            Disponibilidade = Disponibilidade.Livre,
            ValorDeMercado = dadosIA.ValorDeMercado,
            FotoUrl = dadosIA.FotoUrl,
            Visivel = true
        };

        novoJogador.Estatisticas.Add(CriarEstatisticaInicial(novoJogador.Id, dadosIA.Rating));

        var estatistica = novoJogador.Estatisticas.First();
        var jogadorCriado = await _jogadorRepository.CriarAsync(novoJogador, cancellationToken);

        return (201, CriarPayloadJogadorCriado(jogadorCriado, estatistica, dadosIA, consulta));
    }

    public async Task<(int StatusCode, object Payload)> SugerirFiltrosAsync(string descricao, CancellationToken cancellationToken = default)
    {
        var prompt = $@"
Analise a seguinte descrição e sugira filtros para busca de jogadores de CS2:

DESCRIÇÃO: ""{descricao}""

OPÇÕES DISPONÍVEIS:
- Funções: Entry, Suporte, Awp, Igl, Lurker
- Status: Profissional, Aposentado, Amador
- Disponibilidade: EmTime, Livre, Teste
- Países: BR, US, EU, etc.
- Idade máxima: número
- Ordenação: rating_desc, valor_desc, apelido_asc

Retorne um JSON com os filtros sugeridos:
{{
  ""funcao"": ""Awp"",
  ""status"": ""Profissional"",
  ""disp"": ""Livre"",
  ""pais"": ""BR"",
  ""maxIdade"": 25,
  ""ordenar"": ""rating_desc""
}}

Se algum filtro não for aplicável, omita-o do JSON.";

        var response = await GetClient().GetChatCompletionsAsync(new ChatCompletionsOptions
        {
            DeploymentName = "gpt-3.5-turbo",
            Messages =
            {
                new ChatRequestSystemMessage("Você é um especialista em CS2. Responda apenas com JSON válido."),
                new ChatRequestUserMessage(prompt)
            },
            MaxTokens = 300,
            Temperature = 0.1f
        }, cancellationToken);

        var respostaIA = response.Value.Choices[0].Message.Content.Trim();

        try
        {
            var filtrosSugeridos = JsonSerializer.Deserialize<object>(respostaIA);
            return (200, new { filtros = filtrosSugeridos, descricaoOriginal = descricao });
        }
        catch
        {
            return (200, new
            {
                filtros = new { },
                descricaoOriginal = descricao,
                erro = "Não foi possível processar a sugestão de filtros"
            });
        }
    }

    private async Task<JogadorIAResponse> BuscarDadosJogadorPorCampos(string consulta, CancellationToken cancellationToken)
    {
        var tasks = new[]
        {
            BuscarPaisJogador(consulta, cancellationToken),
            BuscarIdadeJogador(consulta, cancellationToken),
            BuscarTimeAtualJogador(consulta, cancellationToken),
            BuscarFuncaoJogador(consulta, cancellationToken),
            BuscarStatusJogador(consulta, cancellationToken),
            BuscarRatingJogador(consulta, cancellationToken),
            BuscarValorMercadoJogador(consulta, cancellationToken),
            Task.FromResult(DefaultFotoUrl)
        };

        var resultados = await Task.WhenAll(tasks);

        return new JogadorIAResponse
        {
            Apelido = consulta,
            Pais = resultados[0],
            Idade = int.TryParse(resultados[1], out var idade) ? idade : 25,
            TimeAtual = string.IsNullOrWhiteSpace(resultados[2]) ? null : resultados[2],
            FuncaoPrincipal = resultados[3],
            Status = resultados[4],
            Rating = decimal.TryParse(resultados[5], out var rating) ? rating : 1.15m,
            ValorDeMercado = decimal.TryParse(resultados[6], out var valor) ? valor : 150000m,
            FotoUrl = resultados[7]
        };
    }

    private async Task<string> BuscarPaisJogador(string consulta, CancellationToken cancellationToken)
    {
        var prompt = $"Qual o país do jogador de CS2 '{consulta}'? Responda apenas com o código do país (ex: BR, US, EU, UA, etc.). Se não souber, responda 'BR'.";
        var response = await FazerPerguntaIA(prompt, 20, cancellationToken);
        return response.Trim().ToUpperInvariant();
    }

    private async Task<string> BuscarIdadeJogador(string consulta, CancellationToken cancellationToken)
    {
        var prompt = $"Qual a idade do jogador de CS2 '{consulta}'? Responda apenas com um número entre 16 e 35. Se não souber, responda '25'.";
        return (await FazerPerguntaIA(prompt, 20, cancellationToken)).Trim();
    }

    private async Task<string> BuscarTimeAtualJogador(string consulta, CancellationToken cancellationToken)
    {
        var prompt = $"Em qual time o jogador de CS2 '{consulta}' joga atualmente? Responda apenas com o nome do time ou 'null' se estiver livre. Se não souber, responda 'null'.";
        var response = await FazerPerguntaIA(prompt, 25, cancellationToken);
        return response.Trim().ToLowerInvariant() == "null" ? "" : response.Trim();
    }

    private async Task<string> BuscarFuncaoJogador(string consulta, CancellationToken cancellationToken)
    {
        var prompt = $"Qual a função principal do jogador de CS2 '{consulta}'? Responda apenas com uma dessas opções: Entry, Suporte, Awp, Igl, Lurker. Se não souber, responda 'Entry'.";
        var funcao = (await FazerPerguntaIA(prompt, 20, cancellationToken)).Trim().Replace(".", "");
        var funcoesValidas = new[] { "Entry", "Suporte", "Awp", "Igl", "Lurker" };
        return funcoesValidas.Contains(funcao) ? funcao : "Entry";
    }

    private async Task<string> BuscarStatusJogador(string consulta, CancellationToken cancellationToken)
    {
        var prompt = $"Qual o status do jogador de CS2 '{consulta}'? Responda apenas com uma dessas opções: Profissional, Aposentado, Amador. Se não souber, responda 'Profissional'.";
        var status = (await FazerPerguntaIA(prompt, 20, cancellationToken)).Trim().Replace(".", "");
        var statusValidos = new[] { "Profissional", "Aposentado", "Amador" };
        return statusValidos.Contains(status) ? status : "Profissional";
    }

    private async Task<string> BuscarRatingJogador(string consulta, CancellationToken cancellationToken)
    {
        var prompt = $"Qual o rating médio do jogador de CS2 '{consulta}'? Responda apenas com um número decimal entre 0.8 e 1.5 (ex: 1.25). Se não souber, responda '1.15'.";
        return (await FazerPerguntaIA(prompt, 20, cancellationToken)).Trim();
    }

    private async Task<string> BuscarValorMercadoJogador(string consulta, CancellationToken cancellationToken)
    {
        var prompt = $"Qual o valor de mercado estimado do jogador de CS2 '{consulta}'? Responda apenas com um número entre 10000 e 5000000 (ex: 150000). Se não souber, responda '150000'.";
        return (await FazerPerguntaIA(prompt, 25, cancellationToken)).Trim();
    }

    private async Task<string> FazerPerguntaIA(string prompt, int maxTokens, CancellationToken cancellationToken)
    {
        var response = await GetClient().GetChatCompletionsAsync(new ChatCompletionsOptions
        {
            DeploymentName = "gpt-3.5-turbo",
            Messages =
            {
                new ChatRequestSystemMessage("Você é um especialista em CS2. Responda APENAS com a informação solicitada, sem explicações, pontos finais ou formatação adicional."),
                new ChatRequestUserMessage(prompt)
            },
            MaxTokens = maxTokens,
            Temperature = 0.1f
        }, cancellationToken);

        return response.Value.Choices[0].Message.Content.Trim();
    }

    private OpenAIClient GetClient()
    {
        var apiKey = _configuration["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("OpenAI API key ausente para integração");
            throw new InvalidOperationException("OpenAI API key não configurada");
        }

        return new OpenAIClient(apiKey);
    }

    private async Task AssociarTimeAsync(Jogador jogador, string? nomeTime, string pais, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(nomeTime))
        {
            return;
        }

        var timeExistente = await _timeRepository.ObterPorNomeAsync(nomeTime, cancellationToken);
        if (timeExistente is not null)
        {
            jogador.TimeAtualId = timeExistente.Id;
            return;
        }

        var novoTime = await _timeRepository.CriarAsync(new Time
        {
            Id = Guid.NewGuid(),
            Nome = nomeTime,
            Pais = pais
        }, cancellationToken);

        jogador.TimeAtualId = novoTime.Id;
    }

    private static Estatistica CriarEstatisticaInicial(Guid jogadorId, decimal rating)
    {
        return new Estatistica
        {
            Id = Guid.NewGuid(),
            JogadorId = jogadorId,
            Periodo = "Geral",
            Rating = rating,
            KD = 1.0m,
            PartidasJogadas = 100
        };
    }

    private static TEnum ParseOuPadrao<TEnum>(string valor, TEnum padrao) where TEnum : struct
    {
        return Enum.TryParse<TEnum>(valor, out var parsed) ? parsed : padrao;
    }

    private static object CriarPayloadJogadorExistente(Jogador jogador, string consulta)
    {
        var estatistica = jogador.Estatisticas.OrderByDescending(e => e.Periodo).FirstOrDefault();
        return new
        {
            jogadores = new[]
            {
                new
                {
                    jogador.Id,
                    jogador.Apelido,
                    jogador.Pais,
                    jogador.Idade,
                    jogador.FuncaoPrincipal,
                    jogador.Status,
                    jogador.Disponibilidade,
                    jogador.ValorDeMercado,
                    jogador.FotoUrl,
                    TimeAtual = jogador.TimeAtual is not null ? new { jogador.TimeAtual.Nome, jogador.TimeAtual.Pais } : null,
                    Rating = estatistica?.Rating ?? 0,
                    KD = estatistica?.KD ?? 0,
                    PartidasJogadas = estatistica?.PartidasJogadas ?? 0
                }
            },
            total = 1,
            consultaOriginal = consulta,
            consultaIA = consulta,
            origem = "banco_dados"
        };
    }

    private static object CriarPayloadJogadorConcorrente(Jogador jogador, string consulta)
    {
        return new
        {
            jogadores = new[]
            {
                new
                {
                    jogador.Id,
                    jogador.Apelido,
                    jogador.Pais,
                    jogador.Idade,
                    jogador.FuncaoPrincipal,
                    jogador.Status,
                    jogador.Disponibilidade,
                    jogador.ValorDeMercado,
                    jogador.FotoUrl,
                    TimeAtual = (object?)null,
                    Rating = 0m,
                    KD = 0m,
                    PartidasJogadas = 0
                }
            },
            total = 1,
            consultaOriginal = consulta,
            consultaIA = consulta,
            origem = "banco_dados_concorrente"
        };
    }

    private static object CriarPayloadJogadorCriado(Jogador jogador, Estatistica estatistica, JogadorIAResponse dadosIA, string consulta)
    {
        return new
        {
            jogadores = new[]
            {
                new
                {
                    jogador.Id,
                    jogador.Apelido,
                    jogador.Pais,
                    jogador.Idade,
                    jogador.FuncaoPrincipal,
                    jogador.Status,
                    jogador.Disponibilidade,
                    jogador.ValorDeMercado,
                    jogador.FotoUrl,
                    TimeAtual = jogador.TimeAtualId is not null ? new { Nome = dadosIA.TimeAtual, Pais = dadosIA.Pais } : null,
                    Rating = estatistica.Rating,
                    KD = estatistica.KD,
                    PartidasJogadas = estatistica.PartidasJogadas
                }
            },
            total = 1,
            consultaOriginal = consulta,
            consultaIA = consulta,
            origem = "ia_criado"
        };
    }
}
