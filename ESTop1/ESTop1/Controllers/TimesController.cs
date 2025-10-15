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
    /// Lista todos os times com filtros
    /// </summary>
    [HttpGet]
    [Authorize]
    [RequerAssinatura("buscar_times")]
    public async Task<IActionResult> Listar([FromQuery] FiltroTime filtros, CancellationToken ct)
    {
        try
        {
            var resultado = await _timeService.ListarTimesAsync(filtros, ct);
            return Ok(resultado);
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
    /// Obtém o time da organização logada
    /// </summary>
    [HttpGet("meu-time")]
    [AuthorizeOrganizacao]
    public async Task<IActionResult> MeuTime(CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized();

            var time = await _timeService.ObterTimePorUsuarioIdAsync(userIdGuid, ct);
            
            if (time == null)
                return NotFound("Time não encontrado para esta organização");

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

