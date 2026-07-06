using ESTop1.Domain;
using ESTop1.Domain.Interfaces;

namespace ESTop1.Infrastructure.Services;

public class AssinaturaService : IAssinaturaService
{
    private readonly IAssinaturaRepository _assinaturaRepository;
    private readonly IPlanoRepository _planoRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public AssinaturaService(
        IAssinaturaRepository assinaturaRepository,
        IPlanoRepository planoRepository,
        IUsuarioRepository usuarioRepository)
    {
        _assinaturaRepository = assinaturaRepository;
        _planoRepository = planoRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Assinatura> CriarAssinaturaAsync(Guid usuarioId, PlanoAssinatura plano)
    {
        var assinaturasAtivas = await _assinaturaRepository.ListarAtivasPorUsuarioAsync(usuarioId);
        foreach (var assinatura in assinaturasAtivas)
        {
            assinatura.Status = StatusAssinatura.Cancelada;
            assinatura.DataCancelamento = DateTime.UtcNow;
            await _assinaturaRepository.AtualizarAsync(assinatura);
        }

        var planoDetalhes = await _planoRepository.ObterAtivoPorTipoAsync(plano);
        if (planoDetalhes is null)
        {
            throw new ArgumentException("Plano não encontrado ou inativo");
        }

        var novaAssinatura = new Assinatura
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Plano = plano,
            Status = StatusAssinatura.Ativa,
            DataInicio = DateTime.UtcNow,
            DataFim = DateTime.UtcNow.AddMonths(1),
            ValorMensal = planoDetalhes.ValorMensal,
            IdTransacao = Guid.NewGuid().ToString()
        };

        return await _assinaturaRepository.CriarAsync(novaAssinatura);
    }

    public Task<Assinatura?> ObterAssinaturaAtivaAsync(Guid usuarioId)
    {
        return _assinaturaRepository.ObterAtivaPorUsuarioAsync(usuarioId);
    }

    public async Task<bool> VerificarAcessoAsync(Guid usuarioId, string recurso)
    {
        var assinatura = await ObterAssinaturaAtivaAsync(usuarioId);
        if (assinatura is null) return false;

        var plano = await _planoRepository.ObterPorTipoAsync(assinatura.Plano);
        if (plano is null) return false;

        return recurso switch
        {
            "estatisticas" => plano.AcessoEstatisticas,
            "busca_ia" => plano.AcessoBuscaIA,
            "api" => plano.AcessoAPI,
            "suporte" => plano.SuportePrioritario,
            "buscar_times" => true,
            "buscar_jogadores" => true,
            _ => false
        };
    }

    public async Task CancelarAssinaturaAsync(Guid assinaturaId)
    {
        var assinatura = await _assinaturaRepository.ObterPorIdAsync(assinaturaId);
        if (assinatura is null) return;

        assinatura.Status = StatusAssinatura.Cancelada;
        assinatura.DataCancelamento = DateTime.UtcNow;
        await _assinaturaRepository.AtualizarAsync(assinatura);
    }

    public async Task RenovarAssinaturaAsync(Guid assinaturaId)
    {
        var assinatura = await _assinaturaRepository.ObterPorIdAsync(assinaturaId);
        if (assinatura is null) return;

        assinatura.DataFim = assinatura.DataFim.AddMonths(1);
        assinatura.Status = StatusAssinatura.Ativa;
        await _assinaturaRepository.AtualizarAsync(assinatura);
    }

    public Task<List<Plano>> ObterPlanosDisponiveisAsync()
    {
        return _planoRepository.ListarAtivosAsync();
    }

    public async Task<Assinatura> AtualizarAssinaturaPorEmailAsync(string email, PlanoAssinatura plano)
    {
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
        if (usuario is null)
        {
            throw new ArgumentException("Usuário não encontrado com o email fornecido");
        }

        return await CriarAssinaturaAsync(usuario.Id, plano);
    }
}
