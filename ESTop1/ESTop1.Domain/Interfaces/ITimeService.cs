using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

/// <summary>
/// Interface para serviço de times
/// </summary>
public interface ITimeService
{
    Task<object> ListarTimesAsync(object filtros, CancellationToken cancellationToken = default);
    Task<object?> ObterTimePorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<object?> ObterTimePorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<object> CriarTimeAsync(object request, CancellationToken cancellationToken = default);
    Task<object?> AtualizarTimeAsync(Guid usuarioId, object request, CancellationToken cancellationToken = default);
}
