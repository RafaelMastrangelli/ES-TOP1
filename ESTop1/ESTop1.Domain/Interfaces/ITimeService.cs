using ESTop1.Domain.DTOs;

namespace ESTop1.Domain.Interfaces;

/// <summary>
/// Interface para serviço de times
/// </summary>
public interface ITimeService
{
    Task<TimesPaginadosDto> ListarTimesAsync(FiltroTime filtros, CancellationToken cancellationToken = default);
    Task<TimeDetalheDto?> ObterTimePorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TimeDetalheDto?> ObterTimePorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<TimeDetalheDto> CriarTimeAsync(Guid usuarioId, CriarTimeCommand request, CancellationToken cancellationToken = default);
    Task<TimeDetalheDto?> AtualizarTimeAsync(Guid usuarioId, AtualizarTimeCommand request, CancellationToken cancellationToken = default);
}
