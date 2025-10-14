using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

/// <summary>
/// Interface para repositório de times
/// </summary>
public interface ITimeRepository
{
    Task<IEnumerable<Time>> ListarAsync(CancellationToken cancellationToken = default);
    Task<Time?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Time?> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default);
    Task<Time> CriarAsync(Time time, CancellationToken cancellationToken = default);
    Task<Time> AtualizarAsync(Time time, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
}
