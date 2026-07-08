using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ESTop1.Api.DTOs;
using Xunit;

namespace ESTop1.Api.Tests;

public static class IntegrationTestHelper
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<LoginResponse> RegistrarEAutenticarAsync(HttpClient client)
    {
        var email = $"user-{Guid.NewGuid():N}@test.com";
        const string senha = "senha123";

        var registroResponse = await client.PostAsJsonAsync("/api/auth/registro", new
        {
            Nome = "Usuário Teste",
            Email = email,
            Senha = senha,
            Tipo = "Organizacao"
        });

        registroResponse.EnsureSuccessStatusCode();

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Senha = senha
        });

        loginResponse.EnsureSuccessStatusCode();

        var login = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        Assert.NotNull(login);
        Assert.False(string.IsNullOrWhiteSpace(login!.Token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login.Token);

        return login;
    }
}
