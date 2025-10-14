using ESTop1.Api.DTOs;
using ESTop1.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESTop1.Api.Controllers;

/// <summary>
/// Controller para gerenciar inscrições de aspirantes
/// </summary>
[ApiController]
[Route("api/inscricoes")]
public class InscricoesController : ControllerBase
{
    private readonly IInscricaoService _inscricaoService;
    
    public InscricoesController(IInscricaoService inscricaoService)
    {
        _inscricaoService = inscricaoService;
    }

    /// <summary>
    /// Cria uma nova inscrição de aspirante
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarInscricao([FromBody] CriarInscricaoRequest request, CancellationToken ct)
    {
        try
        {
            var resultado = await _inscricaoService.CriarInscricaoAsync(request, ct);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar inscrição: {ex.Message}");
        }
    }

    /// <summary>
    /// Simula pagamento da inscrição (mock)
    /// </summary>
    [HttpPost("{id}/pagar")]
    public async Task<IActionResult> PagarInscricao(Guid id, CancellationToken ct)
    {
        try
        {
            var resultado = await _inscricaoService.PagarInscricaoAsync(id, ct);
            return Ok(resultado);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao processar pagamento: {ex.Message}");
        }
    }

    /// <summary>
    /// Aprova uma inscrição (torna jogador visível)
    /// </summary>
    [HttpPost("{id}/aprovar")]
    public async Task<IActionResult> AprovarInscricao(Guid id, CancellationToken ct)
    {
        try
        {
            var resultado = await _inscricaoService.AprovarInscricaoAsync(id, ct);
            return Ok(resultado);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao aprovar inscrição: {ex.Message}");
        }
    }
}

