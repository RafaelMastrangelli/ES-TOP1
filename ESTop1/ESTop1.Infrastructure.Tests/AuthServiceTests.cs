using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ESTop1.Infrastructure.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository = new();
    private readonly IConfiguration _configuration;
    private readonly AuthService _service;
    private readonly List<RefreshToken> _refreshTokens = [];
    private Usuario? _usuarioAtual;

    public AuthServiceTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "ESTop1_Test_Key_For_Unit_Tests_Min32Chars!",
                ["Jwt:Issuer"] = "ESTop1",
                ["Jwt:Audience"] = "ESTop1Client",
                ["Jwt:AccessTokenMinutes"] = "15",
                ["Jwt:RefreshTokenDays"] = "7"
            })
            .Build();

        _refreshTokenRepository
            .Setup(r => r.CriarAsync(It.IsAny<RefreshToken>()))
            .Callback<RefreshToken>(token => _refreshTokens.Add(token))
            .ReturnsAsync((RefreshToken token) => token);

        _refreshTokenRepository
            .Setup(r => r.ObterAtivoPorHashAsync(It.IsAny<string>()))
            .ReturnsAsync((string hash) =>
            {
                var token = _refreshTokens.LastOrDefault(t => t.TokenHash == hash && t.RevokedAt is null);
                if (token is null) return null;

                return new RefreshToken
                {
                    Id = token.Id,
                    TokenHash = token.TokenHash,
                    UsuarioId = token.UsuarioId,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    Usuario = _usuarioAtual ?? new Usuario
                    {
                        Id = token.UsuarioId,
                        Nome = "Usuário Teste",
                        Email = "teste@email.com",
                        Tipo = TipoUsuario.Organizacao,
                        Ativo = true
                    }
                };
            });

        _refreshTokenRepository
            .Setup(r => r.AtualizarAsync(It.IsAny<RefreshToken>()))
            .Callback<RefreshToken>(token =>
            {
                var index = _refreshTokens.FindIndex(t => t.Id == token.Id);
                if (index >= 0) _refreshTokens[index] = token;
            })
            .Returns(Task.CompletedTask);

        _service = new AuthService(
            _usuarioRepository.Object,
            _refreshTokenRepository.Object,
            _configuration,
            NullLogger<AuthService>.Instance);
    }

    [Fact]
    public async Task EmitirTokensAsync_RetornaAccessERefreshToken()
    {
        var usuario = CriarUsuario();
        _usuarioAtual = usuario;

        var resultado = await _service.EmitirTokensAsync(usuario);

        Assert.False(string.IsNullOrWhiteSpace(resultado.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(resultado.RefreshToken));
        Assert.True(resultado.AccessTokenExpiresAt > DateTime.UtcNow);
        Assert.Single(_refreshTokens);
    }

    [Fact]
    public async Task RenovarAcessoAsync_ComTokenInvalido_RetornaNull()
    {
        var resultado = await _service.RenovarAcessoAsync("token-invalido");

        Assert.Null(resultado);
    }

    [Fact]
    public async Task RenovarAcessoAsync_ComTokenValido_RetornaNovosTokens()
    {
        var usuario = CriarUsuario();
        _usuarioAtual = usuario;
        var tokens = await _service.EmitirTokensAsync(usuario);

        var renovado = await _service.RenovarAcessoAsync(tokens.RefreshToken);

        Assert.NotNull(renovado);
        Assert.NotEqual(tokens.RefreshToken, renovado!.RefreshToken);
        Assert.False(string.IsNullOrWhiteSpace(renovado.AccessToken));
    }

    [Fact]
    public async Task RevogarRefreshTokenAsync_InvalidaToken()
    {
        var usuario = CriarUsuario();
        _usuarioAtual = usuario;
        var tokens = await _service.EmitirTokensAsync(usuario);

        await _service.RevogarRefreshTokenAsync(tokens.RefreshToken);

        var renovado = await _service.RenovarAcessoAsync(tokens.RefreshToken);
        Assert.Null(renovado);
    }

    [Fact]
    public async Task ValidarCredenciaisAsync_ComSenhaCorreta_RetornaUsuario()
    {
        const string senha = "senha123";
        var usuario = CriarUsuario();
        usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

        _usuarioRepository
            .Setup(r => r.ObterPorEmailAsync(usuario.Email))
            .ReturnsAsync(usuario);

        _usuarioRepository
            .Setup(r => r.AtualizarAsync(It.IsAny<Usuario>()))
            .ReturnsAsync((Usuario u) => u);

        var resultado = await _service.ValidarCredenciaisAsync(usuario.Email, senha);

        Assert.NotNull(resultado);
        Assert.Equal(usuario.Id, resultado!.Id);
    }

    private static Usuario CriarUsuario() => new()
    {
        Id = Guid.NewGuid(),
        Nome = "Usuário Teste",
        Email = "teste@email.com",
        SenhaHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
        Tipo = TipoUsuario.Organizacao,
        Ativo = true
    };
}
