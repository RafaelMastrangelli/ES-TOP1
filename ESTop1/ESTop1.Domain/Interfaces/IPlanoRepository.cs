using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

public interface IPlanoRepository
{
    Task<Plano?> ObterAtivoPorTipoAsync(PlanoAssinatura tipo, CancellationToken cancellationToken = default);
    Task<Plano?> ObterPorTipoAsync(PlanoAssinatura tipo, CancellationToken cancellationToken = default);
    Task<List<Plano>> ListarAtivosAsync(CancellationToken cancellationToken = default);
}
