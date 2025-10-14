using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

/// <summary>
/// Interface para serviço de jogadores
/// </summary>
public interface IJogadorService
{
    Task<(IEnumerable<object> jogadores, int total)> ListarJogadoresAsync(object filtro, CancellationToken cancellationToken = default);
    Task<object?> ObterJogadorPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<object> CriarJogadorAsync(object request, CancellationToken cancellationToken = default);
    Task<bool> AlterarVisibilidadeJogadorAsync(Guid id, bool visivel, CancellationToken cancellationToken = default);
    Task<object> AtualizarFotosJogadoresAsync(CancellationToken cancellationToken = default);
}
