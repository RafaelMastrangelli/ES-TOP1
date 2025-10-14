using ESTop1.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESTop1.Api.Controllers;

[ApiController]
[Route("api/integracoes/faceit")]
public class FaceitController : ControllerBase
{
    private readonly IFaceitService _faceit;

    public FaceitController(IFaceitService faceit)
    {
        _faceit = faceit;
    }

    /// <summary>
    /// Busca informações de um jogador FACEIT pelo nickname
    /// </summary>
    /// <param name="nickname">Nickname do jogador na FACEIT</param>
    /// <returns>Dados do jogador FACEIT</returns>
    [HttpGet("jogador/{nickname}")]
    public async Task<IActionResult> GetJogador(string nickname)
    {
        var jogador = await _faceit.ObterJogadorPorNicknameAsync(nickname);
        return jogador is null ? NotFound($"Jogador '{nickname}' não encontrado na FACEIT") : Ok(jogador);
    }

    /// <summary>
    /// Busca estatísticas de um jogador FACEIT pelo ID
    /// </summary>
    /// <param name="playerId">ID do jogador na FACEIT</param>
    /// <returns>Estatísticas do jogador</returns>
    [HttpGet("estatisticas/{playerId}")]
    public async Task<IActionResult> GetEstatisticas(string playerId)
    {
        var stats = await _faceit.ObterEstatisticasDoJogadorAsync(playerId);
        return stats is null ? NotFound($"Estatísticas do jogador '{playerId}' não encontradas") : Ok(stats);
    }

    /// <summary>
    /// Busca as últimas partidas de um jogador FACEIT
    /// </summary>
    /// <param name="playerId">ID do jogador na FACEIT</param>
    /// <param name="limite">Número máximo de partidas a retornar (padrão: 5)</param>
    /// <returns>Lista das últimas partidas</returns>
    [HttpGet("partidas/{playerId}")]
    public async Task<IActionResult> GetPartidas(string playerId, int limite = 5)
    {
        var partidas = await _faceit.ObterUltimasPartidasAsync(playerId, limite);
        return Ok(partidas);
    }
}
