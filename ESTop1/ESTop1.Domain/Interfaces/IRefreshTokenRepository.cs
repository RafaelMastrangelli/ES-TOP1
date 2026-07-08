using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> ObterAtivoPorHashAsync(string tokenHash);
    Task<RefreshToken> CriarAsync(RefreshToken refreshToken);
    Task AtualizarAsync(RefreshToken refreshToken);
    Task RevogarTodosDoUsuarioAsync(Guid usuarioId);
}
