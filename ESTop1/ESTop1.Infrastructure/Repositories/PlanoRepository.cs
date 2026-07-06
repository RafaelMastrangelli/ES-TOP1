using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Infrastructure.Repositories;

public class PlanoRepository : IPlanoRepository
{
    private readonly AppDbContext _context;

    public PlanoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Plano?> ObterAtivoPorTipoAsync(PlanoAssinatura tipo, CancellationToken cancellationToken = default)
    {
        return _context.Planos
            .FirstOrDefaultAsync(p => p.Tipo == tipo && p.Ativo, cancellationToken);
    }

    public Task<Plano?> ObterPorTipoAsync(PlanoAssinatura tipo, CancellationToken cancellationToken = default)
    {
        return _context.Planos
            .FirstOrDefaultAsync(p => p.Tipo == tipo, cancellationToken);
    }

    public async Task<List<Plano>> ListarAtivosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Planos
            .Where(p => p.Ativo)
            .OrderBy(p => p.ValorMensal)
            .ToListAsync(cancellationToken);
    }
}
