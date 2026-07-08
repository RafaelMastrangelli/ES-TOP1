using ESTop1.Domain;
using ESTop1.Domain.DTOs;
using ESTop1.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ESTop1.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthTokenResult> EmitirTokensAsync(Usuario usuario)
    {
        var accessTokenMinutes = _configuration.GetValue("Jwt:AccessTokenMinutes", 15);
        var refreshTokenDays = _configuration.GetValue("Jwt:RefreshTokenDays", 7);
        var expiresAt = DateTime.UtcNow.AddMinutes(accessTokenMinutes);

        var accessToken = GerarAccessToken(usuario, expiresAt);
        var refreshToken = GerarRefreshToken();
        var refreshTokenHash = HashToken(refreshToken);

        await _refreshTokenRepository.CriarAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
            CreatedAt = DateTime.UtcNow
        });

        return new AuthTokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = expiresAt
        };
    }

    public async Task<AuthTokenResult?> RenovarAcessoAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);
        var storedToken = await _refreshTokenRepository.ObterAtivoPorHashAsync(tokenHash);

        if (storedToken is null || !storedToken.IsActive)
        {
            _logger.LogWarning("Tentativa de refresh com token inválido ou expirado");
            return null;
        }

        var usuario = storedToken.Usuario;
        if (!usuario.Ativo)
        {
            return null;
        }

        storedToken.RevokedAt = DateTime.UtcNow;

        var accessTokenMinutes = _configuration.GetValue("Jwt:AccessTokenMinutes", 15);
        var refreshTokenDays = _configuration.GetValue("Jwt:RefreshTokenDays", 7);
        var expiresAt = DateTime.UtcNow.AddMinutes(accessTokenMinutes);

        var newAccessToken = GerarAccessToken(usuario, expiresAt);
        var newRefreshToken = GerarRefreshToken();
        var newRefreshTokenHash = HashToken(newRefreshToken);

        storedToken.ReplacedByTokenHash = newRefreshTokenHash;
        await _refreshTokenRepository.AtualizarAsync(storedToken);

        await _refreshTokenRepository.CriarAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            TokenHash = newRefreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
            CreatedAt = DateTime.UtcNow
        });

        return new AuthTokenResult
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            AccessTokenExpiresAt = expiresAt
        };
    }

    public async Task RevogarRefreshTokenAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);
        var storedToken = await _refreshTokenRepository.ObterAtivoPorHashAsync(tokenHash);

        if (storedToken is null || storedToken.RevokedAt is not null)
        {
            return;
        }

        storedToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.AtualizarAsync(storedToken);
    }

    public async Task<Usuario?> ValidarCredenciaisAsync(string email, string senha)
    {
        try
        {
            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash)) return null;

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

    private string GerarAccessToken(Usuario usuario, DateTime expiresAt)
    {
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
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GerarRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private static string HashToken(string token)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
