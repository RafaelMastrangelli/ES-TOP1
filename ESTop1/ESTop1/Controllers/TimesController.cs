using ESTop1.Api.Attributes;
using ESTop1.Api.DTOs;
using ESTop1.Api.Middleware;
using ESTop1.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESTop1.Api.Controllers;

/// <summary>
/// Controller para gerenciar times de CS2
/// </summary>
[ApiController]
[Route("api/times")]
public class TimesController : ControllerBase
{
    private readonly ITimeService _timeService;
    
    public TimesController(ITimeService timeService)
    {
        _timeService = timeService;
    }

    /// <summary>
    /// Lista todos os times
    /// </summary>
    [HttpGet]
    [Authorize]
    [RequerAssinatura("buscar_times")]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        try
        {
            var times = await _timeService.ListarTimesAsync(ct);
            return Ok(times);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao listar times: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém detalhes de um time específico
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    [RequerAssinatura("buscar_times")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        try
        {
            var time = await _timeService.ObterTimePorIdAsync(id, ct);
            
            if (time == null)
                return NotFound();

            return Ok(time);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao obter time: {ex.Message}");
        }
    }

    /// <summary>
    /// Cria um novo time
    /// </summary>
    [HttpPost]
    [AuthorizeOrganizacao]
    [RequerAssinatura("gerenciar_jogadores")]
    public async Task<IActionResult> Criar([FromBody] CriarTimeRequest request, CancellationToken ct)
    {
        try
        {
            var resultado = await _timeService.CriarTimeAsync(request, ct);
            return CreatedAtAction(nameof(Obter), new { id = ((dynamic)resultado).Id }, resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar time: {ex.Message}");
        }
    }
}

