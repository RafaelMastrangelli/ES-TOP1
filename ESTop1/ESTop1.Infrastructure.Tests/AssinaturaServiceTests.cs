using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure.Services;
using Moq;
using Xunit;

namespace ESTop1.Infrastructure.Tests;

public class AssinaturaServiceTests
{
    private readonly Mock<IAssinaturaRepository> _assinaturaRepository = new();
    private readonly Mock<IPlanoRepository> _planoRepository = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly AssinaturaService _service;

    public AssinaturaServiceTests()
    {
        _service = new AssinaturaService(
            _assinaturaRepository.Object,
            _planoRepository.Object,
            _usuarioRepository.Object);
    }

    [Fact]
    public async Task CriarAssinaturaAsync_ComPlanoPago_LancaExcecao()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CriarAssinaturaAsync(Guid.NewGuid(), PlanoAssinatura.Mensal));
    }

    [Fact]
    public async Task CriarAssinaturaAsync_ComPlanoGratuito_CriaAssinatura()
    {
        var usuarioId = Guid.NewGuid();
        var plano = CriarPlano(PlanoAssinatura.Gratuito, 0);

        _assinaturaRepository
            .Setup(r => r.ListarAtivasPorUsuarioAsync(usuarioId, default))
            .ReturnsAsync([]);

        _planoRepository
            .Setup(r => r.ObterAtivoPorTipoAsync(PlanoAssinatura.Gratuito, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plano);

        _assinaturaRepository
            .Setup(r => r.CriarAsync(It.IsAny<Assinatura>(), default))
            .ReturnsAsync((Assinatura a, CancellationToken _) => a);

        var assinatura = await _service.CriarAssinaturaAsync(usuarioId, PlanoAssinatura.Gratuito);

        Assert.Equal(PlanoAssinatura.Gratuito, assinatura.Plano);
        Assert.Equal(StatusAssinatura.Ativa, assinatura.Status);
        Assert.Equal(plano.ValorMensal, assinatura.ValorMensal);
    }

    [Fact]
    public async Task AtivarAssinaturaPorPagamentoAsync_Trimestral_DuracaoTresMeses()
    {
        var usuarioId = Guid.NewGuid();
        var plano = CriarPlano(PlanoAssinatura.Trimestral, 230);

        _assinaturaRepository
            .Setup(r => r.ListarAtivasPorUsuarioAsync(usuarioId, default))
            .ReturnsAsync([]);

        _planoRepository
            .Setup(r => r.ObterAtivoPorTipoAsync(PlanoAssinatura.Trimestral, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plano);

        _assinaturaRepository
            .Setup(r => r.CriarAsync(It.IsAny<Assinatura>(), default))
            .ReturnsAsync((Assinatura a, CancellationToken _) => a);

        var assinatura = await _service.AtivarAssinaturaPorPagamentoAsync(
            usuarioId,
            PlanoAssinatura.Trimestral,
            "mp-123");

        var duracaoEsperada = assinatura.DataFim - assinatura.DataInicio;
        Assert.InRange(duracaoEsperada.TotalDays, 89, 93);
    }

    [Fact]
    public async Task VerificarAcessoAsync_SemAssinatura_RetornaFalse()
    {
        var usuarioId = Guid.NewGuid();

        _assinaturaRepository
            .Setup(r => r.ObterAtivaPorUsuarioAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Assinatura?)null);

        var temAcesso = await _service.VerificarAcessoAsync(usuarioId, "busca_ia");

        Assert.False(temAcesso);
    }

    [Fact]
    public async Task VerificarAcessoAsync_PlanoMensal_PermiteBuscaIA()
    {
        var usuarioId = Guid.NewGuid();
        var assinatura = new Assinatura
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Plano = PlanoAssinatura.Mensal,
            Status = StatusAssinatura.Ativa
        };

        _assinaturaRepository
            .Setup(r => r.ObterAtivaPorUsuarioAsync(usuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assinatura);

        _planoRepository
            .Setup(r => r.ObterPorTipoAsync(PlanoAssinatura.Mensal, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarPlano(PlanoAssinatura.Mensal, 100));

        var temAcesso = await _service.VerificarAcessoAsync(usuarioId, "busca_ia");

        Assert.True(temAcesso);
    }

    private static Plano CriarPlano(PlanoAssinatura tipo, decimal valor) => new()
    {
        Id = Guid.NewGuid(),
        Tipo = tipo,
        Nome = tipo.ToString(),
        Descricao = $"Plano {tipo}",
        ValorMensal = valor,
        LimiteJogadores = tipo == PlanoAssinatura.Trimestral ? -1 : 50,
        AcessoEstatisticas = true,
        AcessoBuscaIA = tipo != PlanoAssinatura.Gratuito,
        AcessoAPI = tipo == PlanoAssinatura.Trimestral,
        SuportePrioritario = tipo == PlanoAssinatura.Trimestral,
        Ativo = true
    };
}
