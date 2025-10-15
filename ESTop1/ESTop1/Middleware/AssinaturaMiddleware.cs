using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using System.Security.Claims;

namespace ESTop1.Api.Middleware;

public class AssinaturaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AssinaturaMiddleware> _logger;

    public AssinaturaMiddleware(RequestDelegate next, ILogger<AssinaturaMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAssinaturaService assinaturaService)
    {
        // Verificar se o endpoint requer assinatura
        var endpoint = context.GetEndpoint();
        var requerAssinatura = endpoint?.Metadata.GetMetadata<RequerAssinaturaAttribute>();
        
        if (requerAssinatura != null)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            var tipoUsuarioClaim = context.User.FindFirst("TipoUsuario");
            
            if (userIdClaim == null || tipoUsuarioClaim == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token de autenticação inválido");
                return;
            }

            var userId = Guid.Parse(userIdClaim.Value);
            var tipoUsuario = Enum.Parse<TipoUsuario>(tipoUsuarioClaim.Value);

            // Verificar regras de acesso baseadas no tipo de usuário
            var temAcesso = await VerificarAcessoPorTipoUsuario(assinaturaService, userId, tipoUsuario, requerAssinatura.Recurso);

            if (!temAcesso)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync($"Acesso negado. Recurso '{requerAssinatura.Recurso}' não disponível para seu tipo de usuário ou requer assinatura ativa.");
                return;
            }
        }

        await _next(context);
    }

    private async Task<bool> VerificarAcessoPorTipoUsuario(IAssinaturaService assinaturaService, Guid userId, TipoUsuario tipoUsuario, string recurso)
    {
        // Administradores têm acesso total
        if (tipoUsuario == TipoUsuario.Admin)
            return true;

        // Regras específicas por tipo de usuário
        switch (tipoUsuario)
        {
            case TipoUsuario.Organizacao:
                // Organizações podem acessar recursos de gerenciamento de jogadores
                if (recurso == "gerenciar_jogadores" || recurso == "buscar_jogadores" || recurso == "estatisticas")
                    return await assinaturaService.VerificarAcessoAsync(userId, recurso);
                break;

            case TipoUsuario.Jogador:
                // Jogadores têm acesso limitado
                if (recurso == "perfil_proprio" || recurso == "estatisticas_proprias")
                    return true; // Sempre permitido para o próprio perfil
                if (recurso == "buscar_times")
                    return true; // Acesso básico a times para jogadores
                if (recurso == "aplicar_vagas")
                    return await assinaturaService.VerificarAcessoAsync(userId, recurso);
                break;
        }

        // Para outros recursos, verificar assinatura normalmente
        return await assinaturaService.VerificarAcessoAsync(userId, recurso);
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequerAssinaturaAttribute : Attribute
{
    public string Recurso { get; }

    public RequerAssinaturaAttribute(string recurso)
    {
        Recurso = recurso;
    }
}
