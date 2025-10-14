using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Infrastructure.Services;

public class AssinaturaService : IAssinaturaService
{
    private readonly AppDbContext _context;

    public AssinaturaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Assinatura> CriarAssinaturaAsync(Guid usuarioId, PlanoAssinatura plano)
    {
        // Cancelar assinaturas ativas existentes
        var assinaturasAtivas = await _context.Assinaturas
            .Where(a => a.UsuarioId == usuarioId && a.Status == StatusAssinatura.Ativa)
            .ToListAsync();

        foreach (var assinatura in assinaturasAtivas)
        {
            assinatura.Status = StatusAssinatura.Cancelada;
            assinatura.DataCancelamento = DateTime.UtcNow;
        }

        // Obter plano
        var planoDetalhes = await _context.Planos
            .FirstOrDefaultAsync(p => p.Tipo == plano && p.Ativo);

        if (planoDetalhes == null)
            throw new ArgumentException("Plano não encontrado ou inativo");

        // Criar nova assinatura
        var novaAssinatura = new Assinatura
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Plano = plano,
            Status = StatusAssinatura.Ativa,
            DataInicio = DateTime.UtcNow,
            DataFim = DateTime.UtcNow.AddMonths(1),
            ValorMensal = planoDetalhes.ValorMensal,
            IdTransacao = Guid.NewGuid().ToString() // Mock - substituir por ID real do gateway
        };

        _context.Assinaturas.Add(novaAssinatura);
        await _context.SaveChangesAsync();

        return novaAssinatura;
    }

    public async Task<Assinatura?> ObterAssinaturaAtivaAsync(Guid usuarioId)
    {
        return await _context.Assinaturas
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.UsuarioId == usuarioId && 
                                     a.Status == StatusAssinatura.Ativa && 
                                     a.DataFim > DateTime.UtcNow);
    }

    public async Task<bool> VerificarAcessoAsync(Guid usuarioId, string recurso)
    {
        var assinatura = await ObterAssinaturaAtivaAsync(usuarioId);
        if (assinatura == null) return false;

        var plano = await _context.Planos
            .FirstOrDefaultAsync(p => p.Tipo == assinatura.Plano);

        if (plano == null) return false;

        return recurso switch
        {
            "estatisticas" => plano.AcessoEstatisticas,
            "busca_ia" => plano.AcessoBuscaIA,
            "api" => plano.AcessoAPI,
            "suporte" => plano.SuportePrioritario,
            _ => false
        };
    }

    public async Task CancelarAssinaturaAsync(Guid assinaturaId)
    {
        var assinatura = await _context.Assinaturas.FindAsync(assinaturaId);
        if (assinatura == null) return;

        assinatura.Status = StatusAssinatura.Cancelada;
        assinatura.DataCancelamento = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RenovarAssinaturaAsync(Guid assinaturaId)
    {
        var assinatura = await _context.Assinaturas.FindAsync(assinaturaId);
        if (assinatura == null) return;

        assinatura.DataFim = assinatura.DataFim.AddMonths(1);
        assinatura.Status = StatusAssinatura.Ativa;

        await _context.SaveChangesAsync();
    }

    public async Task<List<Plano>> ObterPlanosDisponiveisAsync()
    {
        return await _context.Planos
            .Where(p => p.Ativo)
            .OrderBy(p => p.ValorMensal)
            .ToListAsync();
    }

    public async Task<Assinatura> AtualizarAssinaturaPorEmailAsync(string email, PlanoAssinatura plano)
    {
        // Buscar usuário por email
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null)
            throw new ArgumentException("Usuário não encontrado com o email fornecido");

        // Usar o método existente para criar/atualizar a assinatura
        return await CriarAssinaturaAsync(usuario.Id, plano);
    }
}
