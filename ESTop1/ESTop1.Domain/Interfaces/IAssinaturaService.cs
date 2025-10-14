using ESTop1.Domain;

namespace ESTop1.Domain.Interfaces;

public interface IAssinaturaService
{
    Task<Assinatura> CriarAssinaturaAsync(Guid usuarioId, PlanoAssinatura plano);
    Task<Assinatura?> ObterAssinaturaAtivaAsync(Guid usuarioId);
    Task<bool> VerificarAcessoAsync(Guid usuarioId, string recurso);
    Task CancelarAssinaturaAsync(Guid assinaturaId);
    Task RenovarAssinaturaAsync(Guid assinaturaId);
    Task<List<Plano>> ObterPlanosDisponiveisAsync();
    Task<Assinatura> AtualizarAssinaturaPorEmailAsync(string email, PlanoAssinatura plano);
}
