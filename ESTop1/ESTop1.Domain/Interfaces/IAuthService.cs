using ESTop1.Domain;
using ESTop1.Domain.DTOs;

namespace ESTop1.Domain.Interfaces;

public interface IAuthService
{
    Task<AuthTokenResult> EmitirTokensAsync(Usuario usuario);
    Task<AuthTokenResult?> RenovarAcessoAsync(string refreshToken);
    Task RevogarRefreshTokenAsync(string refreshToken);
    Task<Usuario?> ValidarCredenciaisAsync(string email, string senha);
    Task<Usuario> CriarUsuarioAsync(string nome, string email, string senha, TipoUsuario tipo);
    Task<bool> VerificarEmailExisteAsync(string email);
}
