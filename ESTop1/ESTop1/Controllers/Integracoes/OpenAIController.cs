using ESTop1.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.AI.OpenAI;
using System.Text.Json;
using ESTop1.DTOs;
using ESTop1.Domain;

namespace ESTop1.Controllers.Integracoes;

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
    public async Task<IActionResult> BuscarJogadores([FromQuery] string consulta)
    {
        if (string.IsNullOrWhiteSpace(consulta))
        {
            return BadRequest("Consulta não pode estar vazia");
        }

        try
        {
            // Buscar jogador no banco
            var jogadorExistente = await _db.Jogadores
                .AsNoTracking()
                .Include(j => j.TimeAtual)
                .Include(j => j.Estatisticas)
                .FirstOrDefaultAsync(j => j.Apelido.ToLower() == consulta.ToLower() && j.Visivel);

            if (jogadorExistente != null)
            {
                // Jogador existe no banco - retorna 200 OK
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

            // Jogador não existe - usar IA para obter dados
            var prompt = $@"
Você é um especialista em Counter-Strike 2 e jogadores profissionais. 
Busque informações REAIS sobre o jogador '{consulta}' incluindo fotos de sites oficiais como HLTV, Liquipedia, Twitter, Instagram.
Retorne os dados em formato JSON.

INSTRUÇÕES:
1. Busque informações REAIS sobre o jogador '{consulta}' (se for um jogador famoso)
2. Para fotoUrl, use URLs realistas baseadas em padrões conhecidos:
   - HLTV: https://www.hltv.org/img/player/[apelido].jpg
   - Liquipedia: https://liquipedia.net/counterstrike/images/[apelido].jpg
   - Placeholder realista: https://via.placeholder.com/300x300/1a1a1a/ffffff?text=[apelido]
   - Ou use: https://images.unsplash.com/photo-[random]?w=300&h=300&fit=crop
3. Se não encontrar informações reais, retorne dados fictícios realistas
4. Retorne APENAS um JSON com os seguintes campos:
{{
  ""apelido"": ""{consulta}"",
  ""pais"": ""BR"",
  ""idade"": 25,
  ""timeAtual"": ""Nome do Time ou null"",
  ""funcaoPrincipal"": ""Entry"",
  ""status"": ""Profissional"",
  ""rating"": 1.15,
  ""valorDeMercado"": 150000,
  ""fotoUrl"": ""https://via.placeholder.com/300x300/1a1a1a/ffffff?text={consulta}""
}}

CAMPOS OBRIGATÓRIOS:
- apelido: string (use o apelido fornecido)
- pais: string (código do país, ex: ""BR"", ""US"", ""EU"")
- idade: number (entre 16 e 35)
- timeAtual: string ou null (nome do time atual ou null se livre)
- funcaoPrincipal: string (""Entry"", ""Suporte"", ""Awp"", ""Igl"", ""Lurker"")
- status: string (""Profissional"", ""Aposentado"", ""Amador"")
- rating: number (entre 0.8 e 1.5)
- valorDeMercado: number (entre 10000 e 5000000)
- fotoUrl: string (URL da foto do jogador - use padrões como HLTV, placeholder personalizado, ou Unsplash)

RESPOSTA (apenas o JSON):";

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                DeploymentName = "gpt-3.5-turbo",
                Messages =
                {
                    new ChatRequestSystemMessage("Você é um especialista em CS2. Responda apenas com JSON válido."),
                    new ChatRequestUserMessage(prompt)
                },
                MaxTokens = 500,
                Temperature = 0.3f
            };

            var response = await _openAI.GetChatCompletionsAsync(chatCompletionsOptions);
            var respostaIA = response.Value.Choices[0].Message.Content.Trim();

            // Parsear resposta da IA
            JogadorIAResponse dadosIA;
            try
            {
                dadosIA = JsonSerializer.Deserialize<JogadorIAResponse>(respostaIA) ?? throw new Exception("Resposta inválida da IA");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Erro ao processar resposta da IA", detalhes = ex.Message });
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
                Disponibilidade = string.IsNullOrEmpty(dadosIA.TimeAtual) ? Disponibilidade.Livre : Disponibilidade.EmTime,
                ValorDeMercado = dadosIA.ValorDeMercado,
                FotoUrl = GerarFotoUrl(dadosIA.Apelido),
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
    /// Gera uma URL de foto personalizada baseada no apelido do jogador
    /// </summary>
    private static string GerarFotoUrl(string apelido)
    {
        // Usar placeholder personalizado com o apelido do jogador
        var apelidoEncoded = Uri.EscapeDataString(apelido);
        return $"https://via.placeholder.com/300x300/1a1a1a/ffffff?text={apelidoEncoded}";
    }
}
