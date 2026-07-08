using System.Net;
using System.Net.Http.Json;
using ESTop1.Api.DTOs;
using Xunit;

namespace ESTop1.Api.Tests;

[Collection("ApiIntegration")]
public class AuthIntegrationTests
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Registro_Login_Refresh_RetornaNovosTokens()
    {
        var email = $"auth-{Guid.NewGuid():N}@test.com";
        const string senha = "senha123";

        var registroResponse = await _client.PostAsJsonAsync("/api/auth/registro", new
        {
            Nome = "Usuário Integração",
            Email = email,
            Senha = senha,
            Tipo = "Organizacao"
        });

        Assert.Equal(HttpStatusCode.OK, registroResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Senha = senha
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(IntegrationTestHelper.JsonOptions);
        Assert.NotNull(login);
        Assert.False(string.IsNullOrWhiteSpace(login!.Token));
        Assert.False(string.IsNullOrWhiteSpace(login.RefreshToken));

        var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh", new
        {
            RefreshToken = login.RefreshToken
        });

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);

        var refresh = await refreshResponse.Content.ReadFromJsonAsync<LoginResponse>(IntegrationTestHelper.JsonOptions);
        Assert.NotNull(refresh);
        Assert.NotEqual(login.RefreshToken, refresh!.RefreshToken);
        Assert.False(string.IsNullOrWhiteSpace(refresh.Token));
    }

    [Fact]
    public async Task Login_ComCredenciaisInvalidas_RetornaUnauthorized()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = "inexistente@test.com",
            Senha = "senhaerrada"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

[CollectionDefinition("ApiIntegration", DisableParallelization = true)]
public class ApiIntegrationCollectionDefinition : ICollectionFixture<CustomWebApplicationFactory>;
