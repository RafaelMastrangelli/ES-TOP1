using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

/// <summary>
/// Interface para serviço de times
/// </summary>
public interface ITimeService
{
    Task<IEnumerable<object>> ListarTimesAsync(CancellationToken cancellationToken = default);
    Task<object?> ObterTimePorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<object> CriarTimeAsync(object request, CancellationToken cancellationToken = default);
}
