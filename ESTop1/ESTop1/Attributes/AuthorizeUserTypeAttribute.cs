using ESTop1.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ESTop1.Api.Attributes;

/// <summary>
/// Atributo para autorizar acesso baseado no tipo de usuário
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeUserTypeAttribute : Attribute, IAuthorizationFilter
{
    private readonly TipoUsuario[] _tiposPermitidos;

    public AuthorizeUserTypeAttribute(params TipoUsuario[] tiposPermitidos)
    {
        _tiposPermitidos = tiposPermitidos;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Verificar se o usuário está autenticado
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Obter o tipo de usuário do token
        var tipoUsuarioClaim = context.HttpContext.User.FindFirst("TipoUsuario");
        if (tipoUsuarioClaim == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        // Verificar se o tipo de usuário está nos tipos permitidos
        if (!Enum.TryParse<TipoUsuario>(tipoUsuarioClaim.Value, out var tipoUsuario) ||
            !_tiposPermitidos.Contains(tipoUsuario))
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}

/// <summary>
/// Atributo para autorizar apenas organizações/times
/// </summary>
public class AuthorizeOrganizacaoAttribute : AuthorizeUserTypeAttribute
{
    public AuthorizeOrganizacaoAttribute() : base(TipoUsuario.Organizacao, TipoUsuario.Admin) { }
}

/// <summary>
/// Atributo para autorizar apenas jogadores
/// </summary>
public class AuthorizeJogadorAttribute : AuthorizeUserTypeAttribute
{
    public AuthorizeJogadorAttribute() : base(TipoUsuario.Jogador, TipoUsuario.Admin) { }
}

/// <summary>
/// Atributo para autorizar apenas administradores
/// </summary>
public class AuthorizeAdminAttribute : AuthorizeUserTypeAttribute
{
    public AuthorizeAdminAttribute() : base(TipoUsuario.Admin) { }
}
