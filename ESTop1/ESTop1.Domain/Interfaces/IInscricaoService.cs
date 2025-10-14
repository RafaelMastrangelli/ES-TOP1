using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

/// <summary>
/// Interface para serviço de inscrições
/// </summary>
public interface IInscricaoService
{
    Task<object> CriarInscricaoAsync(object request, CancellationToken cancellationToken = default);
    Task<object> PagarInscricaoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<object> AprovarInscricaoAsync(Guid id, CancellationToken cancellationToken = default);
}
