using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de jogadores
/// </summary>
public class JogadorService : IJogadorService
{
    private readonly IJogadorRepository _jogadorRepository;
    private readonly ITimeRepository _timeRepository;
    private readonly AppDbContext _context;

    public JogadorService(IJogadorRepository jogadorRepository, ITimeRepository timeRepository, AppDbContext context)
    {
        _jogadorRepository = jogadorRepository;
        _timeRepository = timeRepository;
        _context = context;
    }

    public async Task<(IEnumerable<object> jogadores, int total)> ListarJogadoresAsync(object filtroObj, CancellationToken cancellationToken = default)
    {
        var filtro = (dynamic)filtroObj;
        var jogadores = await _jogadorRepository.ListarAsync(filtro, cancellationToken);
        var total = await _jogadorRepository.ContarAsync(filtro, cancellationToken);

        var jogadoresFormatados = new List<object>();
        foreach (var j in jogadores)
        {
            decimal ratingGeral = 0;
            foreach (var estatistica in j.Estatisticas)
            {
                if (estatistica.Periodo == "Geral")
                {
                    ratingGeral = estatistica.Rating;
                    break;
                }
            }
            
            jogadoresFormatados.Add(new
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
                RatingGeral = ratingGeral
            });
        }

        return (jogadoresFormatados, total);
    }

    public async Task<object?> ObterJogadorPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var jogador = await _jogadorRepository.ObterPorIdAsync(id, cancellationToken);
        
        if (jogador == null) return null;

        return new
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
    }

    public async Task<object> CriarJogadorAsync(object requestObj, CancellationToken cancellationToken = default)
    {
        var request = (dynamic)requestObj;
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

        var jogadorCriado = await _jogadorRepository.CriarAsync(jogador, cancellationToken);

        return new
        {
            jogadorCriado.Id,
            jogadorCriado.Apelido,
            jogadorCriado.Pais,
            jogadorCriado.Idade,
            jogadorCriado.FuncaoPrincipal,
            jogadorCriado.Status,
            jogadorCriado.Disponibilidade,
            jogadorCriado.ValorDeMercado,
            jogadorCriado.Visivel,
            TimeAtual = jogadorCriado.TimeAtual != null ? new
            {
                jogadorCriado.TimeAtual.Id,
                jogadorCriado.TimeAtual.Nome,
                jogadorCriado.TimeAtual.Pais
            } : null
        };
    }

    public async Task<bool> AlterarVisibilidadeJogadorAsync(Guid id, bool visivel, CancellationToken cancellationToken = default)
    {
        return await _jogadorRepository.AlterarVisibilidadeAsync(id, visivel, cancellationToken);
    }

    public async Task<object> AtualizarFotosJogadoresAsync(CancellationToken cancellationToken = default)
    {
        var jogadores = await _context.Jogadores.ToListAsync(cancellationToken);
        
        foreach (var jogador in jogadores)
        {
            if (string.IsNullOrEmpty(jogador.FotoUrl))
            {
                jogador.FotoUrl = $"https://via.placeholder.com/300x300/1a1a1a/ffffff?text={Uri.EscapeDataString(jogador.Apelido)}";
            }
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return new { message = $"Fotos atualizadas para {jogadores.Count} jogadores" };
    }
}
