using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

public interface IAuthService
{
    Task<string> GerarTokenAsync(Usuario usuario);
    Task<Usuario?> ValidarCredenciaisAsync(string email, string senha);
    Task<Usuario> CriarUsuarioAsync(string nome, string email, string senha, TipoUsuario tipo);
    Task<bool> VerificarEmailExisteAsync(string email);
}
