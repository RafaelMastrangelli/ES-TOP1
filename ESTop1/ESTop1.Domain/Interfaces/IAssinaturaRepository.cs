using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

public interface IAssinaturaRepository
{
    Task<List<Assinatura>> ListarAtivasPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<Assinatura?> ObterAtivaPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<Assinatura?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Assinatura> CriarAsync(Assinatura assinatura, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Assinatura assinatura, CancellationToken cancellationToken = default);
}
