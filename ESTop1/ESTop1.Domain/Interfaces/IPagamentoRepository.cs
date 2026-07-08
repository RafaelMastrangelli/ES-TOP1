using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

public interface IPagamentoRepository
{
    Task<Pagamento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Pagamento?> ObterPorIdExternoAsync(string idExterno, CancellationToken cancellationToken = default);
    Task<List<Pagamento>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<Pagamento> CriarAsync(Pagamento pagamento, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Pagamento pagamento, CancellationToken cancellationToken = default);
}
