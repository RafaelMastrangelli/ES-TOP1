using ESTop1.Api.DTOs;
using ESTop1.Domain;
using ESTop1.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Api.Controllers;

/// <summary>
/// Controller para gerenciar times de CS2
/// </summary>
[ApiController]
[Route("api/times")]
public class TimesController : ControllerBase
{
    private readonly AppDbContext _db;
    public TimesController(AppDbContext db) => _db = db;

    /// <summary>
    /// Lista todos os times
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct)
    {
        var times = await _db.Times.AsNoTracking()
            .Include(t => t.Jogadores)
            .Select(t => new
            {
                t.Id,
                t.Nome,
                t.Pais,
                QuantidadeJogadores = t.Jogadores.Count
            })
            .ToListAsync(ct);

        return Ok(times);
    }

    /// <summary>
    /// Obtém detalhes de um time específico
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var time = await _db.Times.AsNoTracking()
            .Include(t => t.Jogadores)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (time == null)
            return NotFound();

        return Ok(time);
    }

    /// <summary>
    /// Cria um novo time
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarTimeRequest request, CancellationToken ct)
    {
        try
        {
            var time = new Time
            {
                Id = Guid.NewGuid(),
                Nome = request.Nome,
                Pais = request.Pais ?? "BR"
            };

            _db.Times.Add(time);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(Obter), new { id = time.Id }, time);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar time: {ex.Message}");
        }
    }
}

