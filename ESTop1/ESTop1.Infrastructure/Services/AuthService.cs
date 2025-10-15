using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ESTop1.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    public async Task<string> GerarTokenAsync(Usuario usuario)
    {
        try
        {
            Console.WriteLine($"AuthService: Gerando token para usuário: {usuario.Nome}");
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim("user_id", usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Tipo.ToString()),
                new Claim("TipoUsuario", usuario.Tipo.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"AuthService: Token gerado com sucesso");
            
            return tokenString;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AuthService: Erro ao gerar token: {ex.Message}");
            throw;
        }
    }

    public async Task<Usuario?> ValidarCredenciaisAsync(string email, string senha)
    {
        try
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash)) return null;

            // Atualizar último login
            usuario.UltimoLogin = DateTime.UtcNow;
            await _usuarioRepository.AtualizarAsync(usuario);

            return usuario;
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException("Erro ao validar credenciais", ex);
        }
    }

    public async Task<Usuario> CriarUsuarioAsync(string nome, string email, string senha, TipoUsuario tipo)
    {
        try
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = nome,
                Email = email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
                Tipo = tipo,
                Ativo = true
            };

            return await _usuarioRepository.CriarAsync(usuario);
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Erro ao criar usuário: {ex.Message}", ex);
        }
    }

    public async Task<bool> VerificarEmailExisteAsync(string email)
    {
        return await _usuarioRepository.ExisteEmailAsync(email);
    }
}
