using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de times
/// </summary>
public class TimeRepository : ITimeRepository
{
    private readonly AppDbContext _context;

    public TimeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Time>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Times.AsNoTracking()
            .Include(t => t.Jogadores)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Time>> ListarAsync(FiltroTime filtro, CancellationToken cancellationToken = default)
    {
        return await AplicarFiltros(_context.Times.AsNoTracking().Include(t => t.Jogadores), filtro)
            .Skip((filtro.Page - 1) * filtro.PageSize)
            .Take(filtro.PageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> ContarAsync(FiltroTime filtro, CancellationToken cancellationToken = default)
    {
        return AplicarFiltros(_context.Times.AsNoTracking(), filtro, aplicarOrdenacao: false)
            .CountAsync(cancellationToken);
    }

    public async Task<Time?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Times.AsNoTracking()
            .Include(t => t.Jogadores)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Time?> ObterRastreadoPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Times
            .Include(t => t.Jogadores)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Time?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        return await _context.Times.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Nome.ToLower() == nome.ToLower(), cancellationToken);
    }

    public async Task<Time> CriarAsync(Time time, CancellationToken cancellationToken = default)
    {
        _context.Times.Add(time);
        await _context.SaveChangesAsync(cancellationToken);
        return time;
    }

    public async Task<Time> AtualizarAsync(Time time, CancellationToken cancellationToken = default)
    {
        _context.Times.Update(time);
        await _context.SaveChangesAsync(cancellationToken);
        return time;
    }

    public async Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var time = await _context.Times.FindAsync([id], cancellationToken);
        if (time == null) return false;

        _context.Times.Remove(time);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<Time> AplicarFiltros(IQueryable<Time> query, FiltroTime filtro, bool aplicarOrdenacao = true)
    {
        if (!string.IsNullOrEmpty(filtro.Nome))
        {
            query = query.Where(t => t.Nome.Contains(filtro.Nome));
        }

        if (filtro.Tier.HasValue)
        {
            query = query.Where(t => t.Tier == filtro.Tier.Value);
        }

        if (filtro.Contratando.HasValue)
        {
            query = query.Where(t => t.Contratando == filtro.Contratando.Value);
        }

        if (!aplicarOrdenacao)
        {
            return query;
        }

        return filtro.Ordenar switch
        {
            "nome_asc" => query.OrderBy(t => t.Nome),
            "tier_asc" => query.OrderBy(t => t.Tier ?? int.MaxValue),
            "tier_desc" => query.OrderByDescending(t => t.Tier ?? 0),
            _ => query.OrderBy(t => t.Nome)
        };
    }
}
