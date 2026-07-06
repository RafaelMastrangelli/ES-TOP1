using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Infrastructure.Repositories;

public class AssinaturaRepository : IAssinaturaRepository
{
    private readonly AppDbContext _context;

    public AssinaturaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Assinatura>> ListarAtivasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _context.Assinaturas
            .Where(a => a.UsuarioId == usuarioId && a.Status == StatusAssinatura.Ativa)
            .ToListAsync(cancellationToken);
    }

    public async Task<Assinatura?> ObterAtivaPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return await _context.Assinaturas
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a =>
                a.UsuarioId == usuarioId &&
                a.Status == StatusAssinatura.Ativa &&
                a.DataFim > DateTime.UtcNow,
                cancellationToken);
    }

    public async Task<Assinatura?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Assinaturas.FindAsync([id], cancellationToken);
    }

    public async Task<Assinatura> CriarAsync(Assinatura assinatura, CancellationToken cancellationToken = default)
    {
        _context.Assinaturas.Add(assinatura);
        await _context.SaveChangesAsync(cancellationToken);
        return assinatura;
    }

    public async Task AtualizarAsync(Assinatura assinatura, CancellationToken cancellationToken = default)
    {
        _context.Assinaturas.Update(assinatura);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
