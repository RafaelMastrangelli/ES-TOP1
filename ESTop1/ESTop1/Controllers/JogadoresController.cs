using ESTop1.Api.DTOs;
using ESTop1.Domain;
using ESTop1.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Api.Controllers;

/// <summary>
/// Controller para gerenciar jogadores profissionais de CS2
/// </summary>
[ApiController]
[Route("api/jogadores")]
public class JogadoresController : ControllerBase
{
    private readonly AppDbContext _db;
    public JogadoresController(AppDbContext db) => _db = db;

    /// <summary>
    /// Lista jogadores com filtros e paginação
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] FiltroJogador filtro, CancellationToken ct)
    {
        var query = _db.Jogadores.AsNoTracking()
            .Include(j => j.TimeAtual)
            .Include(j => j.Estatisticas)
            .Where(j => j.Visivel);

        // Aplicar filtros
        if (!string.IsNullOrEmpty(filtro.Q))
            query = query.Where(j => j.Apelido.Contains(filtro.Q));

        if (!string.IsNullOrEmpty(filtro.Pais))
            query = query.Where(j => j.Pais == filtro.Pais);

        if (filtro.Status.HasValue)
            query = query.Where(j => j.Status == filtro.Status.Value);

        if (filtro.Disp.HasValue)
            query = query.Where(j => j.Disponibilidade == filtro.Disp.Value);

        if (filtro.Funcao.HasValue)
            query = query.Where(j => j.FuncaoPrincipal == filtro.Funcao.Value);

        if (filtro.MaxIdade.HasValue)
            query = query.Where(j => j.Idade <= filtro.MaxIdade.Value);

        // Aplicar ordenação
        query = filtro.Ordenar switch
        {
            "rating_desc" => query.OrderByDescending(j => j.Estatisticas.Where(e => e.Periodo == "Geral").Select(e => e.Rating).FirstOrDefault()),
            "valor_desc" => query.OrderByDescending(j => j.ValorDeMercado),
            "apelido_asc" => query.OrderBy(j => j.Apelido),
            _ => query.OrderBy(j => j.Apelido)
        };

        // Contar total
        var total = await query.CountAsync(ct);

        // Aplicar paginação
        var jogadores = await query
            .Skip((filtro.Page - 1) * filtro.PageSize)
            .Take(filtro.PageSize)
            .Select(j => new
            {
                j.Id,
                j.Apelido,
                j.Pais,
                j.Idade,
                Time = j.TimeAtual != null ? j.TimeAtual.Nome : null,
                j.FuncaoPrincipal,
                j.Status,
                j.Disponibilidade,
                j.ValorDeMercado,
                j.FotoUrl,
                RatingGeral = j.Estatisticas.Where(e => e.Periodo == "Geral").Select(e => e.Rating).FirstOrDefault()
            })
            .ToListAsync(ct);

        return Ok(new { total, page = filtro.Page, pageSize = filtro.PageSize, items = jogadores });
    }

    /// <summary>
    /// Obtém detalhes de um jogador específico
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var jogador = await _db.Jogadores.AsNoTracking()
            .Include(j => j.TimeAtual)
            .Include(j => j.Estatisticas)
            .FirstOrDefaultAsync(j => j.Id == id, ct);

        if (jogador == null)
            return NotFound();

        // Retornar dados sem referências circulares
        var resultado = new
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
            jogador.Visivel,
            TimeAtual = jogador.TimeAtual != null ? new
            {
                jogador.TimeAtual.Id,
                jogador.TimeAtual.Nome,
                jogador.TimeAtual.Pais
            } : null,
            Estatisticas = jogador.Estatisticas.Select(e => new
            {
                e.Id,
                e.Periodo,
                e.Rating,
                e.KD,
                e.PartidasJogadas
            }).ToList()
        };

        return Ok(resultado);
    }

    /// <summary>
    /// Atualiza as fotos dos jogadores existentes com URLs personalizadas
    /// </summary>
    [HttpPost("atualizar-fotos")]
    public async Task<IActionResult> AtualizarFotos(CancellationToken ct)
    {
        var jogadores = await _db.Jogadores.ToListAsync(ct);
        
        foreach (var jogador in jogadores)
        {
            if (string.IsNullOrEmpty(jogador.FotoUrl))
            {
                jogador.FotoUrl = $"https://via.placeholder.com/300x300/1a1a1a/ffffff?text={Uri.EscapeDataString(jogador.Apelido)}";
            }
        }
        
        await _db.SaveChangesAsync(ct);
        
        return Ok(new { message = $"Fotos atualizadas para {jogadores.Count} jogadores" });
    }

    /// <summary>
    /// Cria um novo jogador
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarJogadorRequest request, CancellationToken ct)
    {
        try
        {
            var jogador = new Jogador
            {
                Id = Guid.NewGuid(),
                Apelido = request.Apelido,
                Pais = request.Pais ?? "BR",
                Idade = request.Idade,
                FuncaoPrincipal = request.FuncaoPrincipal,
                Status = request.Status,
                Disponibilidade = request.Disponibilidade,
                TimeAtualId = request.TimeAtualId,
                ValorDeMercado = request.ValorDeMercado,
                Visivel = true
            };

            _db.Jogadores.Add(jogador);
            await _db.SaveChangesAsync(ct);

            // Retornar dados sem referências circulares
            var resultado = new
            {
                jogador.Id,
                jogador.Apelido,
                jogador.Pais,
                jogador.Idade,
                jogador.FuncaoPrincipal,
                jogador.Status,
                jogador.Disponibilidade,
                jogador.ValorDeMercado,
                jogador.Visivel,
                TimeAtual = jogador.TimeAtual != null ? new
                {
                    jogador.TimeAtual.Id,
                    jogador.TimeAtual.Nome,
                    jogador.TimeAtual.Pais
                } : null
            };

            return CreatedAtAction(nameof(Obter), new { id = jogador.Id }, resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar jogador: {ex.Message}");
        }
    }

    /// <summary>
    /// Altera a visibilidade de um jogador
    /// </summary>
    [HttpPut("{id}/visibilidade")]
    public async Task<IActionResult> AlterarVisibilidade(Guid id, [FromQuery] bool on, CancellationToken ct)
    {
        try
        {
            var jogador = await _db.Jogadores.FindAsync(id);
            if (jogador == null)
                return NotFound();

            jogador.Visivel = on;
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = $"Jogador {(on ? "tornado visível" : "ocultado")} com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao alterar visibilidade: {ex.Message}");
        }
    }
}

