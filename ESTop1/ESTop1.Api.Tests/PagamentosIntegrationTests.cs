using System.Net;
using System.Net.Http.Json;
using ESTop1.Domain.DTOs;
using Xunit;

namespace ESTop1.Api.Tests;

[Collection("ApiIntegration")]
public class PagamentosIntegrationTests
{
    private readonly HttpClient _client;

    public PagamentosIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Checkout_SimularAprovacao_AtivaAssinatura()
    {
        await IntegrationTestHelper.RegistrarEAutenticarAsync(_client);

        var checkoutResponse = await _client.PostAsJsonAsync("/api/pagamentos/checkout", new
        {
            Plano = "Mensal",
            Metodo = "checkout"
        });

        Assert.Equal(HttpStatusCode.OK, checkoutResponse.StatusCode);

        var checkout = await checkoutResponse.Content.ReadFromJsonAsync<CheckoutPagamentoResult>(IntegrationTestHelper.JsonOptions);
        Assert.NotNull(checkout);
        Assert.Equal("Pendente", checkout!.Status);

        var simularResponse = await _client.PostAsync(
            $"/api/pagamentos/{checkout.PagamentoId}/simular-aprovacao",
            null);

        Assert.Equal(HttpStatusCode.OK, simularResponse.StatusCode);

        var simulado = await simularResponse.Content.ReadFromJsonAsync<CheckoutPagamentoResult>(IntegrationTestHelper.JsonOptions);
        Assert.NotNull(simulado);
        Assert.True(simulado!.AprovadoImediatamente);

        var assinaturaResponse = await _client.GetAsync("/api/assinaturas/minha");
        Assert.Equal(HttpStatusCode.OK, assinaturaResponse.StatusCode);

        var assinatura = await assinaturaResponse.Content.ReadFromJsonAsync<AssinaturaResponse>(IntegrationTestHelper.JsonOptions);
        Assert.NotNull(assinatura);
        Assert.Equal("Mensal", assinatura!.Plano);
        Assert.Equal("Ativa", assinatura.Status);
    }

    [Fact]
    public async Task Checkout_PlanoGratuito_AprovaImediatamente()
    {
        await IntegrationTestHelper.RegistrarEAutenticarAsync(_client);

        var checkoutResponse = await _client.PostAsJsonAsync("/api/pagamentos/checkout", new
        {
            Plano = "Gratuito",
            Metodo = "gratuito"
        });

        Assert.Equal(HttpStatusCode.OK, checkoutResponse.StatusCode);

        var checkout = await checkoutResponse.Content.ReadFromJsonAsync<CheckoutPagamentoResult>(IntegrationTestHelper.JsonOptions);
        Assert.NotNull(checkout);
        Assert.True(checkout!.AprovadoImediatamente);
        Assert.Equal("Aprovado", checkout.Status);
    }

    [Fact]
    public async Task ObterPlanos_RetornaPlanosCadastrados()
    {
        await IntegrationTestHelper.RegistrarEAutenticarAsync(_client);

        var response = await _client.GetAsync("/api/assinaturas/planos");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var planos = await response.Content.ReadFromJsonAsync<List<PlanoResponse>>(IntegrationTestHelper.JsonOptions);
        Assert.NotNull(planos);
        Assert.True(planos!.Count >= 3);
        Assert.Contains(planos, p => p.Tipo == "Gratuito");
        Assert.Contains(planos, p => p.Tipo == "Mensal");
        Assert.Contains(planos, p => p.Tipo == "Trimestral");
    }
}

public class PlanoResponse
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public decimal ValorMensal { get; set; }
}

public class AssinaturaResponse
{
    public Guid Id { get; set; }
    public string Plano { get; set; } = null!;
    public string Status { get; set; } = null!;
}
