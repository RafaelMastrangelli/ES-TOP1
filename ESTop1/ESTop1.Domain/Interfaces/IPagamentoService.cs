using ESTop1.Domain;
using ESTop1.Domain.DTOs;

namespace ESTop1.Domain.Interfaces;

public interface IPagamentoService
{
    Task<CheckoutPagamentoResult> IniciarPagamentoAsync(
        Guid usuarioId,
        string email,
        PlanoAssinatura plano,
        string metodo,
        CancellationToken cancellationToken = default);

    Task<PagamentoStatusResult?> ObterStatusAsync(Guid pagamentoId, Guid usuarioId, CancellationToken cancellationToken = default);

    Task<List<PagamentoStatusResult>> ListarHistoricoAsync(Guid usuarioId, CancellationToken cancellationToken = default);

    Task ProcessarNotificacaoMercadoPagoAsync(string paymentId, CancellationToken cancellationToken = default);

    Task<CheckoutPagamentoResult?> SimularAprovacaoDevAsync(Guid pagamentoId, Guid usuarioId, CancellationToken cancellationToken = default);
}
