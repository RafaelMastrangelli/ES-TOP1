using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure;
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

    public async Task<Time?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Times.AsNoTracking()
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
        var time = await _context.Times.FindAsync(id);
        if (time == null) return false;

        _context.Times.Remove(time);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
