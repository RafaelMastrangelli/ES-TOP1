using ESTop1.Api.DTOs;
using ESTop1.Domain;
using ESTop1.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Api.Controllers;

/// <summary>
/// Controller para gerenciar inscrições de aspirantes
/// </summary>
[ApiController]
[Route("api/inscricoes")]
public class InscricoesController : ControllerBase
{
    private readonly AppDbContext _db;
    public InscricoesController(AppDbContext db) => _db = db;

    /// <summary>
    /// Cria uma nova inscrição de aspirante
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CriarInscricao([FromBody] CriarInscricaoRequest request, CancellationToken ct)
    {
        try
        {
            var jogadorId = Guid.NewGuid();
            var jogador = new Jogador
            {
                Id = jogadorId,
                Apelido = request.Apelido,
                Pais = request.Pais ?? "BR",
                Idade = request.Idade,
                FuncaoPrincipal = request.FuncaoPrincipal,
                Status = StatusJogador.Amador,
                Disponibilidade = Disponibilidade.Livre,
                ValorDeMercado = 0,
                Visivel = false
            };

            // Adicionar estatística geral se fornecida
            if (request.Rating.HasValue || request.KD.HasValue || request.PartidasJogadas.HasValue)
            {
                jogador.Estatisticas.Add(new Estatistica
                {
                    Id = Guid.NewGuid(),
                    JogadorId = jogadorId,
                    Periodo = "Geral",
                    Rating = request.Rating ?? 0,
                    KD = request.KD ?? 0,
                    PartidasJogadas = request.PartidasJogadas ?? 0
                });
            }

            _db.Jogadores.Add(jogador);
            await _db.SaveChangesAsync(ct);

            return Ok(new { inscricaoId = jogadorId, message = "Inscrição criada com sucesso" });
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
            var jogador = await _db.Jogadores.FindAsync(id);
            if (jogador == null)
                return NotFound();

            // Mock: apenas retorna sucesso
            return Ok(new { message = "Pagamento processado com sucesso" });
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
            var jogador = await _db.Jogadores.FindAsync(id);
            if (jogador == null)
                return NotFound();

            jogador.Visivel = true;
            await _db.SaveChangesAsync(ct);

            return Ok(new { message = "Inscrição aprovada com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao aprovar inscrição: {ex.Message}");
        }
    }
}

