using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de jogadores
/// </summary>
public class JogadorRepository : IJogadorRepository
{
    private readonly AppDbContext _context;

    public JogadorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Jogador>> ListarAsync(FiltroJogador filtro, CancellationToken cancellationToken = default)
    {
        var query = _context.Jogadores.AsNoTracking()
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

        // Aplicar paginação
        return await query
            .Skip((filtro.Page - 1) * filtro.PageSize)
            .Take(filtro.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Jogador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Jogadores.AsNoTracking()
            .Include(j => j.TimeAtual)
            .Include(j => j.Estatisticas)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task<Jogador?> ObterPorApelidoAsync(string apelido, CancellationToken cancellationToken = default)
    {
        return await _context.Jogadores.AsNoTracking()
            .Include(j => j.TimeAtual)
            .Include(j => j.Estatisticas)
            .FirstOrDefaultAsync(j => j.Apelido.ToLower().Trim() == apelido.ToLower().Trim() && j.Visivel, cancellationToken);
    }

    public async Task<Jogador> CriarAsync(Jogador jogador, CancellationToken cancellationToken = default)
    {
        _context.Jogadores.Add(jogador);
        await _context.SaveChangesAsync(cancellationToken);
        return jogador;
    }

    public async Task<Jogador> AtualizarAsync(Jogador jogador, CancellationToken cancellationToken = default)
    {
        _context.Jogadores.Update(jogador);
        await _context.SaveChangesAsync(cancellationToken);
        return jogador;
    }

    public async Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var jogador = await _context.Jogadores.FindAsync(id);
        if (jogador == null) return false;

        _context.Jogadores.Remove(jogador);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> ContarAsync(FiltroJogador filtro, CancellationToken cancellationToken = default)
    {
        var query = _context.Jogadores.AsNoTracking()
            .Where(j => j.Visivel);

        // Aplicar filtros (mesma lógica do ListarAsync)
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

        return await query.CountAsync(cancellationToken);
    }

    public async Task<bool> AlterarVisibilidadeAsync(Guid id, bool visivel, CancellationToken cancellationToken = default)
    {
        var jogador = await _context.Jogadores.FindAsync(id);
        if (jogador == null) return false;

        jogador.Visivel = visivel;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
