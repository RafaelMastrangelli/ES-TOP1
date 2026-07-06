using ESTop1.Domain.DTOs;

namespace ESTop1.Domain.Interfaces;

/// <summary>
/// Interface para serviço de jogadores
/// </summary>
public interface IJogadorService
{
    Task<JogadoresPaginadosDto> ListarJogadoresAsync(FiltroJogador filtro, CancellationToken cancellationToken = default);
    Task<JogadorDetalheDto?> ObterJogadorPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<JogadorDetalheDto?> ObterJogadorPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
    Task<JogadorResumoDto> CriarJogadorAsync(CriarJogadorCommand request, CancellationToken cancellationToken = default);
    Task<JogadorResumoDto> CriarJogadorParaUsuarioAsync(Guid usuarioId, string nome, CancellationToken cancellationToken = default);
    Task<JogadorDetalheDto?> AtualizarJogadorAsync(Guid usuarioId, AtualizarJogadorCommand request, CancellationToken cancellationToken = default);
    Task<bool> AlterarVisibilidadeJogadorAsync(Guid id, bool visivel, CancellationToken cancellationToken = default);
    Task<AtualizarFotosResultDto> AtualizarFotosJogadoresAsync(CancellationToken cancellationToken = default);
}
