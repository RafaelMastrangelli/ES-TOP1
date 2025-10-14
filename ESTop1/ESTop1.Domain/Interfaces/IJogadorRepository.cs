using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

/// <summary>
/// Interface para repositório de jogadores
/// </summary>
public interface IJogadorRepository
{
    Task<IEnumerable<Jogador>> ListarAsync(FiltroJogador filtro, CancellationToken cancellationToken = default);
    Task<Jogador?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Jogador?> ObterPorApelidoAsync(string apelido, CancellationToken cancellationToken = default);
    Task<Jogador> CriarAsync(Jogador jogador, CancellationToken cancellationToken = default);
    Task<Jogador> AtualizarAsync(Jogador jogador, CancellationToken cancellationToken = default);
    Task<bool> ExcluirAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> ContarAsync(FiltroJogador filtro, CancellationToken cancellationToken = default);
    Task<bool> AlterarVisibilidadeAsync(Guid id, bool visivel, CancellationToken cancellationToken = default);
}
