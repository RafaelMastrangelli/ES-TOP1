using ESTop1.Api.Attributes;
using ESTop1.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESTop1.Api.Controllers.Integracoes;

[ApiController]
[Route("api/integracoes/openai")]
public class OpenAIController : ControllerBase
{
    private readonly IOpenAIService _openAIService;

    public OpenAIController(IOpenAIService openAIService)
    {
        _openAIService = openAIService;
    }

    /// <summary>
    /// Busca um jogador específico no banco. Se não existir, usa IA para obter dados e criar o jogador.
    /// </summary>
    /// <param name="consulta">Consulta/nome do jogador a ser buscado</param>
    /// <returns>Dados do jogador (200 se existir, 201 se criado via IA)</returns>
    [HttpGet("buscar-jogadores")]
    [Authorize]
    [RequerAssinatura("busca_ia")]
    public async Task<IActionResult> BuscarJogadores([FromQuery] string consulta, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(consulta))
        {
            return BadRequest("Consulta não pode estar vazia");
        }

        try
        {
            var (statusCode, payload) = await _openAIService.BuscarJogadoresAsync(consulta, cancellationToken);
            return StatusCode(statusCode, payload);
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
    public async Task<IActionResult> BuscarJogadoresTeste([FromQuery] string consulta, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(consulta))
        {
            return BadRequest("Consulta não pode estar vazia");
        }

        try
        {
            var (statusCode, payload) = await _openAIService.BuscarJogadoresTesteAsync(consulta, cancellationToken);
            return StatusCode(statusCode, payload);
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
    public async Task<IActionResult> SugerirFiltros([FromQuery] string descricao, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            return BadRequest("Descrição não pode estar vazia");
        }

        try
        {
            var (statusCode, payload) = await _openAIService.SugerirFiltrosAsync(descricao, cancellationToken);
            return StatusCode(statusCode, payload);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Erro interno ao processar sugestão", detalhes = ex.Message });
        }
    }
}
