using ESTop1.Infrastructure;
using ESTop1.Api.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.AI.OpenAI;
using System.Text.Json;
using ESTop1.Api.DTOs;
using ESTop1.Domain;

namespace ESTop1.Api.Controllers;

[ApiController]
[Route("api/integracoes/openai")]
public class OpenAIController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly OpenAIClient _openAI;

    public OpenAIController(AppDbContext db, OpenAIClient openAI)
    {
        _db = db;
        _openAI = openAI;
    }

    /// <summary>
    /// Busca um jogador específico no banco. Se não existir, usa IA para obter dados e criar o jogador.
    /// </summary>
    /// <param name="consulta">Consulta/nome do jogador a ser buscado</param>
    /// <returns>Dados do jogador (200 se existir, 201 se criado via IA)</returns>
    [HttpGet("buscar-jogadores")]
    [Authorize]
    [RequerAssinatura("busca_ia")]
    public async Task<IActionResult> BuscarJogadores([FromQuery] string consulta)
    {
        if (string.IsNullOrWhiteSpace(consulta))
        {
            return BadRequest("Consulta não pode estar vazia");
        }

        try
        {
            // Buscar jogador no banco (normalizar consulta)
            var consultaNormalizada = consulta.Trim().ToLower();
            var jogadorExistente = await _db.Jogadores
                .AsNoTracking()
                .Include(j => j.TimeAtual)
                .Include(j => j.Estatisticas)
                .FirstOrDefaultAsync(j => j.Apelido.ToLower().Trim() == consultaNormalizada && j.Visivel);

            if (jogadorExistente != null)
            {
                // Jogador existe no banco - retorna 200 OK (não cria duplicado)
                var estatistica = jogadorExistente.Estatisticas
                    .OrderByDescending(e => e.Periodo)
                    .FirstOrDefault();

                return Ok(new
                {
                    jogadores = new[]
                    {
                        new
                        {
                            jogadorExistente.Id,
                            jogadorExistente.Apelido,
                            jogadorExistente.Pais,
                            jogadorExistente.Idade,
                            jogadorExistente.FuncaoPrincipal,
                            jogadorExistente.Status,
                            jogadorExistente.Disponibilidade,
                            jogadorExistente.ValorDeMercado,
                            jogadorExistente.FotoUrl,
                            TimeAtual = jogadorExistente.TimeAtual != null ? new { jogadorExistente.TimeAtual.Nome, jogadorExistente.TimeAtual.Pais } : null,
                            Rating = estatistica?.Rating ?? 0,
                            KD = estatistica?.KD ?? 0,
                            PartidasJogadas = estatistica?.PartidasJogadas ?? 0
                        }
                    },
                    total = 1,
                    consultaOriginal = consulta,
                    consultaIA = consulta,
                    origem = "banco_dados"
                });
            }

            // Jogador não existe - usar IA para obter dados com perguntas específicas
            JogadorIAResponse dadosIA;
            try
            {
                dadosIA = await BuscarDadosJogadorPorCampos(consulta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao processar dados da IA", detalhes = ex.Message });
            }

            // Verificação adicional: garantir que não foi criado por outra requisição simultânea
            var jogadorConcorrente = await _db.Jogadores
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Apelido.ToLower().Trim() == consultaNormalizada && j.Visivel);
            
            if (jogadorConcorrente != null)
            {
                // Jogador foi criado por outra requisição - retornar o existente
                return Ok(new
                {
                    jogadores = new[]
                    {
                        new
                        {
                            jogadorConcorrente.Id,
                            jogadorConcorrente.Apelido,
                            jogadorConcorrente.Pais,
                            jogadorConcorrente.Idade,
                            jogadorConcorrente.FuncaoPrincipal,
                            jogadorConcorrente.Status,
                            jogadorConcorrente.Disponibilidade,
                            jogadorConcorrente.ValorDeMercado,
                            jogadorConcorrente.FotoUrl,
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
                });
            }

            // Converter para entidade Jogador
            var novoJogador = new Jogador
            {
                Id = Guid.NewGuid(),
                Apelido = dadosIA.Apelido,
                Pais = dadosIA.Pais,
                Idade = dadosIA.Idade,
                FuncaoPrincipal = Enum.Parse<Funcao>(dadosIA.FuncaoPrincipal),
                Status = Enum.Parse<StatusJogador>(dadosIA.Status),
                Disponibilidade = string.IsNullOrEmpty(dadosIA.TimeAtual) ? Disponibilidade.Livre : Disponibilidade.Contratado,
                ValorDeMercado = dadosIA.ValorDeMercado,
                FotoUrl = "/player-default.jpg", // Imagem padrão temporária
                Visivel = true
            };

            // Criar estatística inicial
            var novaEstatistica = new Estatistica
            {
                Id = Guid.NewGuid(),
                JogadorId = novoJogador.Id,
                Periodo = "Geral",
                Rating = dadosIA.Rating,
                KD = 1.0m, // Valor padrão
                PartidasJogadas = 100 // Valor padrão
            };

            // Se tem time, criar ou buscar o time
            if (!string.IsNullOrEmpty(dadosIA.TimeAtual))
            {
                var timeExistente = await _db.Times
                    .FirstOrDefaultAsync(t => t.Nome.ToLower() == dadosIA.TimeAtual.ToLower());

                if (timeExistente != null)
                {
                    novoJogador.TimeAtualId = timeExistente.Id;
                }
                else
                {
                    var novoTime = new Time
                    {
                        Id = Guid.NewGuid(),
                        Nome = dadosIA.TimeAtual,
                        Pais = dadosIA.Pais
                    };
                    _db.Times.Add(novoTime);
                    novoJogador.TimeAtualId = novoTime.Id;
                }
            }

            // Salvar no banco
            _db.Jogadores.Add(novoJogador);
            _db.Estatisticas.Add(novaEstatistica);
            await _db.SaveChangesAsync();

            // Retornar 201 Created
            return StatusCode(201, new
            {
                jogadores = new[]
                {
                    new
                    {
                        novoJogador.Id,
                        novoJogador.Apelido,
                        novoJogador.Pais,
                        novoJogador.Idade,
                        novoJogador.FuncaoPrincipal,
                        novoJogador.Status,
                        novoJogador.Disponibilidade,
                        novoJogador.ValorDeMercado,
                        novoJogador.FotoUrl,
                        TimeAtual = novoJogador.TimeAtualId != null ? new { Nome = dadosIA.TimeAtual, Pais = dadosIA.Pais } : null,
                        Rating = novaEstatistica.Rating,
                        KD = novaEstatistica.KD,
                        PartidasJogadas = novaEstatistica.PartidasJogadas
                    }
                },
                total = 1,
                consultaOriginal = consulta,
                consultaIA = consulta,
                origem = "ia_criado"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro interno ao processar busca do jogador", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// TESTE: Busca jogadores com IA sem verificação de assinatura (apenas para desenvolvimento)
    /// </summary>
    /// <param name="consulta">Consulta/nome do jogador a ser buscado</param>
    /// <returns>Dados do jogador (200 se existir, 201 se criado via IA)</returns>
    [HttpGet("teste/buscar-jogadores")]
    [Authorize]
    public async Task<IActionResult> BuscarJogadoresTeste([FromQuery] string consulta)
    {
        if (string.IsNullOrWhiteSpace(consulta))
        {
            return BadRequest("Consulta não pode estar vazia");
        }

        try
        {
            // Buscar jogador no banco (normalizar consulta)
            var consultaNormalizada = consulta.Trim().ToLower();
            var jogadorExistente = await _db.Jogadores
                .AsNoTracking()
                .Include(j => j.TimeAtual)
                .Include(j => j.Estatisticas)
                .FirstOrDefaultAsync(j => j.Apelido.ToLower().Trim() == consultaNormalizada && j.Visivel);

            if (jogadorExistente != null)
            {
                // Jogador existe no banco - retorna 200 OK (não cria duplicado)
                var estatistica = jogadorExistente.Estatisticas
                    .OrderByDescending(e => e.Periodo)
                    .FirstOrDefault();

                return Ok(new
                {
                    jogadores = new[]
                    {
                        new
                        {
                            jogadorExistente.Id,
                            jogadorExistente.Apelido,
                            jogadorExistente.Pais,
                            jogadorExistente.Idade,
                            jogadorExistente.FuncaoPrincipal,
                            jogadorExistente.Status,
                            jogadorExistente.Disponibilidade,
                            jogadorExistente.ValorDeMercado,
                            jogadorExistente.FotoUrl,
                            TimeAtual = jogadorExistente.TimeAtual != null ? new { jogadorExistente.TimeAtual.Nome, jogadorExistente.TimeAtual.Pais } : null,
                            Rating = estatistica?.Rating ?? 0,
                            KD = estatistica?.KD ?? 0,
                            PartidasJogadas = estatistica?.PartidasJogadas ?? 0
                        }
                    },
                    total = 1,
                    consultaOriginal = consulta,
                    consultaIA = consulta,
                    origem = "banco_dados"
                });
            }

            // Jogador não existe - usar IA para obter dados com perguntas específicas
            JogadorIAResponse dadosIA;
            try
            {
                dadosIA = await BuscarDadosJogadorPorCampos(consulta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao processar dados da IA", detalhes = ex.Message });
            }

            // Verificação adicional: garantir que não foi criado por outra requisição simultânea
            var jogadorConcorrente = await _db.Jogadores
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Apelido.ToLower().Trim() == consultaNormalizada && j.Visivel);
            
            if (jogadorConcorrente != null)
            {
                // Jogador foi criado por outra requisição - retornar o existente
                return Ok(new
                {
                    jogadores = new[]
                    {
                        new
                        {
                            jogadorConcorrente.Id,
                            jogadorConcorrente.Apelido,
                            jogadorConcorrente.Pais,
                            jogadorConcorrente.Idade,
                            jogadorConcorrente.FuncaoPrincipal,
                            jogadorConcorrente.Status,
                            jogadorConcorrente.Disponibilidade,
                            jogadorConcorrente.ValorDeMercado,
                            jogadorConcorrente.FotoUrl,
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
                });
            }

            // Converter para entidade Jogador
            var novoJogador = new Jogador
            {
                Id = Guid.NewGuid(),
                Apelido = consulta.Trim(),
                Pais = dadosIA.Pais,
                Idade = dadosIA.Idade,
                FuncaoPrincipal = Enum.Parse<Funcao>(dadosIA.FuncaoPrincipal),
                Status = Enum.Parse<StatusJogador>(dadosIA.Status),
                Disponibilidade = Disponibilidade.Livre,
                ValorDeMercado = dadosIA.ValorDeMercado,
                FotoUrl = dadosIA.FotoUrl,
                Visivel = true
            };

            // Criar estatística inicial
            var novaEstatistica = new Estatistica
            {
                Id = Guid.NewGuid(),
                JogadorId = novoJogador.Id,
                Periodo = "Geral",
                Rating = dadosIA.Rating,
                KD = 1.0m, // Valor padrão
                PartidasJogadas = 100 // Valor padrão
            };

            // Salvar no banco
            _db.Jogadores.Add(novoJogador);
            _db.Estatisticas.Add(novaEstatistica);
            await _db.SaveChangesAsync();

            // Retornar 201 Created com dados do jogador criado
            return StatusCode(201, new
            {
                jogadores = new[]
                {
                    new
                    {
                        novoJogador.Id,
                        novoJogador.Apelido,
                        novoJogador.Pais,
                        novoJogador.Idade,
                        novoJogador.FuncaoPrincipal,
                        novoJogador.Status,
                        novoJogador.Disponibilidade,
                        novoJogador.ValorDeMercado,
                        novoJogador.FotoUrl,
                        TimeAtual = novoJogador.TimeAtualId != null ? new { Nome = dadosIA.TimeAtual, Pais = dadosIA.Pais } : null,
                        Rating = novaEstatistica.Rating,
                        KD = novaEstatistica.KD,
                        PartidasJogadas = novaEstatistica.PartidasJogadas
                    }
                },
                total = 1,
                consultaOriginal = consulta,
                consultaIA = consulta,
                origem = "ia_criado"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro interno ao processar busca do jogador", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Sugere filtros baseados em uma descrição natural
    /// </summary>
    /// <param name="descricao">Descrição do que o usuário está procurando</param>
    /// <returns>Sugestões de filtros aplicáveis</returns>
    [HttpGet("sugerir-filtros")]
    public async Task<IActionResult> SugerirFiltros([FromQuery] string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            return BadRequest("Descrição não pode estar vazia");
        }

        try
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

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = "gpt-3.5-turbo",
                Messages =
                {
                    new ChatRequestSystemMessage("Você é um especialista em CS2. Responda apenas com JSON válido."),
                    new ChatRequestUserMessage(prompt)
                },
                MaxTokens = 300,
                Temperature = 0.1f
            };

            var response = await _openAI.GetChatCompletionsAsync(chatCompletionsOptions);
            var respostaIA = response.Value.Choices[0].Message.Content.Trim();

            try
            {
                var filtrosSugeridos = JsonSerializer.Deserialize<object>(respostaIA);
                return Ok(new
                {
                    filtros = filtrosSugeridos,
                    descricaoOriginal = descricao
                });
            }
            catch
            {
                return Ok(new
                {
                    filtros = new { },
                    descricaoOriginal = descricao,
                    erro = "Não foi possível processar a sugestão de filtros"
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro interno ao processar sugestão", detalhes = ex.Message });
        }
    }

    /// <summary>
    /// Busca dados do jogador fazendo perguntas específicas para cada campo
    /// </summary>
    private async Task<JogadorIAResponse> BuscarDadosJogadorPorCampos(string consulta)
    {
        // Criar tarefas para buscar cada campo em paralelo
        var tasks = new[]
        {
            BuscarPaisJogador(consulta),
            BuscarIdadeJogador(consulta),
            BuscarTimeAtualJogador(consulta),
            BuscarFuncaoJogador(consulta),
            BuscarStatusJogador(consulta),
            BuscarRatingJogador(consulta),
            BuscarValorMercadoJogador(consulta),
            Task.FromResult("/player-default.jpg") // Foto padrão temporária
        };

        // Aguardar todas as respostas
        var resultados = await Task.WhenAll(tasks);

        // Montar resposta final com tratamento de erro
        return new JogadorIAResponse
        {
            Apelido = consulta,
            Pais = resultados[0],
            Idade = int.TryParse(resultados[1], out var idade) ? idade : 25,
            TimeAtual = string.IsNullOrEmpty(resultados[2]) ? null : resultados[2],
            FuncaoPrincipal = resultados[3],
            Status = resultados[4],
            Rating = decimal.TryParse(resultados[5], out var rating) ? rating : 1.15m,
            ValorDeMercado = decimal.TryParse(resultados[6], out var valor) ? valor : 150000m,
            FotoUrl = resultados[7]
        };
    }

    /// <summary>
    /// Busca o país do jogador
    /// </summary>
    private async Task<string> BuscarPaisJogador(string consulta)
    {
        var prompt = $"Qual o país do jogador de CS2 '{consulta}'? Responda apenas com o código do país (ex: BR, US, EU, UA, etc.). Se não souber, responda 'BR'.";
        
        var response = await FazerPerguntaIA(prompt);
        return response.Trim().ToUpper();
    }

    /// <summary>
    /// Busca a idade do jogador
    /// </summary>
    private async Task<string> BuscarIdadeJogador(string consulta)
    {
        var prompt = $"Qual a idade do jogador de CS2 '{consulta}'? Responda apenas com um número entre 16 e 35. Se não souber, responda '25'.";
        
        var response = await FazerPerguntaIA(prompt);
        return response.Trim();
    }

    /// <summary>
    /// Busca o time atual do jogador
    /// </summary>
    private async Task<string> BuscarTimeAtualJogador(string consulta)
    {
        var prompt = $"Em qual time o jogador de CS2 '{consulta}' joga atualmente? Responda apenas com o nome do time ou 'null' se estiver livre. Se não souber, responda 'null'.";
        
        var response = await FazerPerguntaIA(prompt);
        return response.Trim().ToLower() == "null" ? "" : response.Trim();
    }

    /// <summary>
    /// Busca a função principal do jogador
    /// </summary>
    private async Task<string> BuscarFuncaoJogador(string consulta)
    {
        var prompt = $"Qual a função principal do jogador de CS2 '{consulta}'? Responda apenas com uma dessas opções: Entry, Suporte, Awp, Igl, Lurker. Se não souber, responda 'Entry'.";
        
        var response = await FazerPerguntaIA(prompt);
        var funcao = response.Trim().Replace(".", ""); // Remove pontos
        
        // Validar se é uma função válida
        var funcoesValidas = new[] { "Entry", "Suporte", "Awp", "Igl", "Lurker" };
        return funcoesValidas.Contains(funcao) ? funcao : "Entry";
    }

    /// <summary>
    /// Busca o status do jogador
    /// </summary>
    private async Task<string> BuscarStatusJogador(string consulta)
    {
        var prompt = $"Qual o status do jogador de CS2 '{consulta}'? Responda apenas com uma dessas opções: Profissional, Aposentado, Amador. Se não souber, responda 'Profissional'.";
        
        var response = await FazerPerguntaIA(prompt);
        var status = response.Trim().Replace(".", ""); // Remove pontos
        
        // Validar se é um status válido
        var statusValidos = new[] { "Profissional", "Aposentado", "Amador" };
        return statusValidos.Contains(status) ? status : "Profissional";
    }

    /// <summary>
    /// Busca o rating do jogador
    /// </summary>
    private async Task<string> BuscarRatingJogador(string consulta)
    {
        var prompt = $"Qual o rating médio do jogador de CS2 '{consulta}'? Responda apenas com um número decimal entre 0.8 e 1.5 (ex: 1.25). Se não souber, responda '1.15'.";
        
        var response = await FazerPerguntaIA(prompt);
        return response.Trim();
    }

    /// <summary>
    /// Busca o valor de mercado do jogador
    /// </summary>
    private async Task<string> BuscarValorMercadoJogador(string consulta)
    {
        var prompt = $"Qual o valor de mercado estimado do jogador de CS2 '{consulta}'? Responda apenas com um número entre 10000 e 5000000 (ex: 150000). Se não souber, responda '150000'.";
        
        var response = await FazerPerguntaIA(prompt);
        return response.Trim();
    }

    // Função de busca de foto removida temporariamente - usando imagem padrão

    /// <summary>
    /// Faz uma pergunta específica para a IA
    /// </summary>
    private async Task<string> FazerPerguntaIA(string prompt)
    {
        var chatCompletionsOptions = new ChatCompletionsOptions()
        {
            DeploymentName = "gpt-3.5-turbo",
            Messages =
            {
                new ChatRequestSystemMessage("Você é um especialista em CS2. Responda APENAS com a informação solicitada, sem explicações, pontos finais ou formatação adicional."),
                new ChatRequestUserMessage(prompt)
            },
            MaxTokens = 20,
            Temperature = 0.1f
        };

        var response = await _openAI.GetChatCompletionsAsync(chatCompletionsOptions);
        return response.Value.Choices[0].Message.Content.Trim();
    }

    /// <summary>
    /// Gera uma URL de foto personalizada baseada no apelido do jogador
    /// </summary>
    private static string GerarFotoUrl(string apelido)
    {
        // Usar placeholder personalizado com o apelido do jogador
        var apelidoEncoded = Uri.EscapeDataString(apelido);
        return $"https://via.placeholder.com/300x300/1a1a1a/ffffff?text={apelidoEncoded}";
    }
}
