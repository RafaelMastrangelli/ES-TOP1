using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace ESTop1.Infrastructure.Tests;

public class PagamentoServiceTests
{
    private readonly Mock<IPagamentoRepository> _pagamentoRepository = new();
    private readonly Mock<IPlanoRepository> _planoRepository = new();
    private readonly Mock<IAssinaturaService> _assinaturaService = new();
    private readonly IConfiguration _configurationDev;
    private readonly IConfiguration _configurationProd;

    public PagamentoServiceTests()
    {
        _configurationDev = CriarConfiguration("Development");
        _configurationProd = CriarConfiguration("Production");
    }

    [Fact]
    public async Task IniciarPagamentoAsync_PlanoGratuito_AprovadoImediatamente()
    {
        var usuarioId = Guid.NewGuid();
        var assinaturaId = Guid.NewGuid();
        var plano = CriarPlano(PlanoAssinatura.Gratuito, 0);

        _planoRepository
            .Setup(r => r.ObterAtivoPorTipoAsync(PlanoAssinatura.Gratuito, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plano);

        _assinaturaService
            .Setup(s => s.CriarAssinaturaAsync(usuarioId, PlanoAssinatura.Gratuito))
            .ReturnsAsync(new Assinatura { Id = assinaturaId, Plano = PlanoAssinatura.Gratuito });

        _pagamentoRepository
            .Setup(r => r.CriarAsync(It.IsAny<Pagamento>(), default))
            .ReturnsAsync((Pagamento p, CancellationToken _) => p);

        var service = CriarService(_configurationDev);
        var resultado = await service.IniciarPagamentoAsync(
            usuarioId,
            "teste@email.com",
            PlanoAssinatura.Gratuito,
            "gratuito");

        Assert.True(resultado.AprovadoImediatamente);
        Assert.Equal(StatusPagamento.Aprovado.ToString(), resultado.Status);
        Assert.Equal(assinaturaId, resultado.AssinaturaId);
    }

    [Fact]
    public async Task IniciarPagamentoAsync_PlanoPagoSemMercadoPago_CriaPendenteEmDev()
    {
        var usuarioId = Guid.NewGuid();
        var plano = CriarPlano(PlanoAssinatura.Mensal, 100);

        _planoRepository
            .Setup(r => r.ObterAtivoPorTipoAsync(PlanoAssinatura.Mensal, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plano);

        _pagamentoRepository
            .Setup(r => r.CriarAsync(It.IsAny<Pagamento>(), default))
            .ReturnsAsync((Pagamento p, CancellationToken _) => p);

        var service = CriarService(_configurationDev);
        var resultado = await service.IniciarPagamentoAsync(
            usuarioId,
            "teste@email.com",
            PlanoAssinatura.Mensal,
            "checkout");

        Assert.False(resultado.AprovadoImediatamente);
        Assert.Equal(StatusPagamento.Pendente.ToString(), resultado.Status);
        Assert.Equal(100, resultado.Valor);
    }

    [Fact]
    public async Task SimularAprovacaoDevAsync_ForaDeDevelopment_LancaExcecao()
    {
        var service = CriarService(_configurationProd);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.SimularAprovacaoDevAsync(Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public async Task SimularAprovacaoDevAsync_PagamentoPendente_AprovaPagamento()
    {
        var usuarioId = Guid.NewGuid();
        var pagamentoId = Guid.NewGuid();
        var pagamento = new Pagamento
        {
            Id = pagamentoId,
            UsuarioId = usuarioId,
            Plano = PlanoAssinatura.Mensal,
            Valor = 100,
            Status = StatusPagamento.Pendente,
            MetodoPagamento = "checkout"
        };

        _pagamentoRepository
            .Setup(r => r.ObterPorIdAsync(pagamentoId, default))
            .ReturnsAsync(pagamento);

        _assinaturaService
            .Setup(s => s.AtivarAssinaturaPorPagamentoAsync(
                usuarioId,
                PlanoAssinatura.Mensal,
                It.IsAny<string>()))
            .ReturnsAsync(new Assinatura
            {
                Id = Guid.NewGuid(),
                Plano = PlanoAssinatura.Mensal,
                Status = StatusAssinatura.Ativa
            });

        _pagamentoRepository
            .Setup(r => r.AtualizarAsync(It.IsAny<Pagamento>(), default))
            .Returns(Task.CompletedTask);

        var service = CriarService(_configurationDev);
        var resultado = await service.SimularAprovacaoDevAsync(pagamentoId, usuarioId);

        Assert.NotNull(resultado);
        Assert.True(resultado!.AprovadoImediatamente);
        Assert.Equal(StatusPagamento.Aprovado.ToString(), resultado.Status);
    }

    private PagamentoService CriarService(IConfiguration configuration) => new(
        _pagamentoRepository.Object,
        _planoRepository.Object,
        _assinaturaService.Object,
        new MockHttpClientFactory(),
        configuration,
        NullLogger<PagamentoService>.Instance);

    private static IConfiguration CriarConfiguration(string environment) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = environment,
                ["MercadoPago:AccessToken"] = ""
            })
            .Build();

    private static Plano CriarPlano(PlanoAssinatura tipo, decimal valor) => new()
    {
        Id = Guid.NewGuid(),
        Tipo = tipo,
        Nome = tipo.ToString(),
        Descricao = $"Plano {tipo}",
        ValorMensal = valor,
        Ativo = true
    };

    private sealed class MockHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new HttpClient();
    }
}
