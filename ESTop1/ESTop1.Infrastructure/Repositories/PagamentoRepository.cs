using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Infrastructure.Repositories;

public class PagamentoRepository : IPagamentoRepository
{
    private readonly AppDbContext _context;

    public PagamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Pagamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _context.Pagamentos
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Pagamento?> ObterPorIdExternoAsync(string idExterno, CancellationToken cancellationToken = default)
    {
        return _context.Pagamentos
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.IdExterno == idExterno, cancellationToken);
    }

    public Task<List<Pagamento>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        return _context.Pagamentos
            .Where(p => p.UsuarioId == usuarioId)
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync(cancellationToken);
    }

    public async Task<Pagamento> CriarAsync(Pagamento pagamento, CancellationToken cancellationToken = default)
    {
        _context.Pagamentos.Add(pagamento);
        await _context.SaveChangesAsync(cancellationToken);
        return pagamento;
    }

    public async Task AtualizarAsync(Pagamento pagamento, CancellationToken cancellationToken = default)
    {
        _context.Pagamentos.Update(pagamento);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
