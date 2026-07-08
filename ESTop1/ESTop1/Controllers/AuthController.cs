using ESTop1.Api.DTOs;
using ESTop1.Domain;
using ESTop1.Domain.DTOs;
using ESTop1.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESTop1.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAssinaturaService _assinaturaService;
    private readonly IJogadorService _jogadorService;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        IAssinaturaService assinaturaService,
        IJogadorService jogadorService,
        IUsuarioRepository usuarioRepository,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _assinaturaService = assinaturaService;
        _jogadorService = jogadorService;
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    /// <summary>
    /// Realiza login do usuário
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Validação básica dos dados de entrada
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Email é obrigatório",
                    ErrorCode = "EMAIL_REQUIRED"
                });
            }

            if (string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Senha é obrigatória",
                    ErrorCode = "PASSWORD_REQUIRED"
                });
            }

            // Validação de formato de email
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Formato de email inválido",
                    ErrorCode = "INVALID_EMAIL_FORMAT"
                });
            }

            _logger.LogInformation("Tentativa de login para email: {Email}", request.Email);
            
            var usuario = await _authService.ValidarCredenciaisAsync(request.Email, request.Senha);
            if (usuario == null)
            {
                _logger.LogWarning("Credenciais inválidas para email: {Email}", request.Email);
                return Unauthorized(new ErrorResponse
                {
                    Message = "Email ou senha incorretos",
                    ErrorCode = "INVALID_CREDENTIALS"
                });
            }
            
            _logger.LogInformation("Login bem-sucedido para usuário {UsuarioId}", usuario.Id);

            // Verificar se o usuário está ativo
            if (!usuario.Ativo)
            {
                return Unauthorized(new ErrorResponse
                {
                    Message = "Conta desativada. Entre em contato com o suporte.",
                    ErrorCode = "ACCOUNT_DISABLED"
                });
            }

            var tokens = await _authService.EmitirTokensAsync(usuario);
            var assinatura = await _assinaturaService.ObterAssinaturaAtivaAsync(usuario.Id);

            var response = CriarLoginResponse(usuario, tokens, assinatura);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "VALIDATION_ERROR"
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "AUTH_ERROR"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Message = "Erro interno do servidor. Tente novamente mais tarde.",
                ErrorCode = "INTERNAL_ERROR",
                Details = ex.Message
            });
        }
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroRequest request)
    {
        try
        {
            // Validação básica dos dados de entrada
            if (string.IsNullOrWhiteSpace(request.Nome))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Nome é obrigatório",
                    ErrorCode = "NAME_REQUIRED"
                });
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Email é obrigatório",
                    ErrorCode = "EMAIL_REQUIRED"
                });
            }

            if (string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Senha é obrigatória",
                    ErrorCode = "PASSWORD_REQUIRED"
                });
            }

            // Validação de formato de email
            if (!IsValidEmail(request.Email))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Formato de email inválido",
                    ErrorCode = "INVALID_EMAIL_FORMAT"
                });
            }

            // Validação de senha
            if (request.Senha.Length < 6)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Senha deve ter pelo menos 6 caracteres",
                    ErrorCode = "PASSWORD_TOO_SHORT"
                });
            }

            if (await _authService.VerificarEmailExisteAsync(request.Email))
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Email já está em uso",
                    ErrorCode = "EMAIL_ALREADY_EXISTS"
                });
            }

            // Validar tipo de usuário
            if (!request.IsValidTipo())
            {
                return BadRequest(new ErrorResponse
                {
                    Message = $"Tipo de usuário inválido. Tipos disponíveis: {string.Join(", ", RegistroRequest.GetTiposDisponiveis())}",
                    ErrorCode = "INVALID_USER_TYPE"
                });
            }

            var tipoUsuario = request.GetTipoUsuario();
            
            var usuario = await _authService.CriarUsuarioAsync(
                request.Nome, 
                request.Email, 
                request.Senha, 
                tipoUsuario
            );

            // Criar assinatura gratuita para jogadores
            if (tipoUsuario == TipoUsuario.Jogador)
            {
                await _assinaturaService.CriarAssinaturaAsync(usuario.Id, PlanoAssinatura.Gratuito);
                
                // Criar registro de jogador para usuários do tipo Jogador
                await _jogadorService.CriarJogadorParaUsuarioAsync(usuario.Id, request.Nome);
            }
            // Organizações começam com plano gratuito mas com limitações
            else if (tipoUsuario == TipoUsuario.Organizacao)
            {
                await _assinaturaService.CriarAssinaturaAsync(usuario.Id, PlanoAssinatura.Gratuito);
            }

            var tokens = await _authService.EmitirTokensAsync(usuario);
            var assinatura = await _assinaturaService.ObterAssinaturaAtivaAsync(usuario.Id);

            var response = CriarLoginResponse(usuario, tokens, assinatura);

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ErrorResponse
            {
                Message = ex.Message,
                ErrorCode = "VALIDATION_ERROR"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse
            {
                Message = "Erro interno do servidor. Tente novamente mais tarde.",
                ErrorCode = "INTERNAL_ERROR",
                Details = ex.Message
            });
        }
    }

    /// <summary>
    /// Obtém informações do usuário logado
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> ObterUsuarioAtual()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var assinatura = await _assinaturaService.ObterAssinaturaAtivaAsync(userId);

            var response = new
            {
                usuario = new UsuarioResponse
                {
                    Id = userId,
                    Nome = User.FindFirst(System.Security.Claims.ClaimTypes.Name)!.Value,
                    Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)!.Value,
                    Tipo = User.FindFirst("TipoUsuario")!.Value
                },
                assinatura = assinatura != null ? new AssinaturaResponse
                {
                    Id = assinatura.Id,
                    Plano = assinatura.Plano.ToString(),
                    Status = assinatura.Status.ToString(),
                    DataInicio = assinatura.DataInicio,
                    DataFim = assinatura.DataFim,
                    ValorMensal = assinatura.ValorMensal
                } : null
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter dados do usuário autenticado");
            return BadRequest($"Erro ao obter dados do usuário: {ex.Message}");
        }
    }

    /// <summary>
    /// Renova o access token usando refresh token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest(new ErrorResponse
            {
                Message = "Refresh token é obrigatório",
                ErrorCode = "REFRESH_TOKEN_REQUIRED"
            });
        }

        var tokens = await _authService.RenovarAcessoAsync(request.RefreshToken);
        if (tokens is null)
        {
            return Unauthorized(new ErrorResponse
            {
                Message = "Refresh token inválido ou expirado",
                ErrorCode = "INVALID_REFRESH_TOKEN"
            });
        }

        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var usuarioId = Guid.Parse(
            handler.ReadJwtToken(tokens.AccessToken).Claims
                .First(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);

        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario is null)
        {
            return Unauthorized(new ErrorResponse
            {
                Message = "Usuário não encontrado",
                ErrorCode = "USER_NOT_FOUND"
            });
        }

        var assinatura = await _assinaturaService.ObterAssinaturaAtivaAsync(usuarioId);
        return Ok(CriarLoginResponse(usuario, tokens, assinatura));
    }

    /// <summary>
    /// Revoga o refresh token atual (logout)
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            await _authService.RevogarRefreshTokenAsync(request.RefreshToken);
        }

        return Ok(new { message = "Logout realizado com sucesso" });
    }

    /// <summary>
    /// Obtém informações sobre tipos de usuário disponíveis
    /// </summary>
    [HttpGet("tipos-usuario")]
    public IActionResult ObterTiposUsuario()
    {
        var tipos = new[]
        {
            new
            {
                Tipo = "Jogador",
                Nome = "Jogador",
                Descricao = "Jogador individual que pode buscar times e aplicar para vagas",
                Recursos = new[] { "Perfil próprio", "Buscar times", "Aplicar vagas", "Estatísticas próprias" }
            },
            new
            {
                Tipo = "Organizacao",
                Nome = "Organização/Time",
                Descricao = "Organização ou time que pode gerenciar jogadores e buscar talentos",
                Recursos = new[] { "Gerenciar jogadores", "Buscar jogadores", "Estatísticas avançadas", "API de integração" }
            }
        };

        return Ok(tipos);
    }

    /// <summary>
    /// Valida se o formato do email está correto
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static LoginResponse CriarLoginResponse(Usuario usuario, AuthTokenResult tokens, Assinatura? assinatura)
    {
        return new LoginResponse
        {
            Token = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            ExpiresAt = tokens.AccessTokenExpiresAt,
            Usuario = new UsuarioResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Tipo = usuario.Tipo.ToString(),
                DataCriacao = usuario.DataCriacao,
                UltimoLogin = usuario.UltimoLogin
            },
            Assinatura = assinatura != null ? new AssinaturaResponse
            {
                Id = assinatura.Id,
                Plano = assinatura.Plano.ToString(),
                Status = assinatura.Status.ToString(),
                DataInicio = assinatura.DataInicio,
                DataFim = assinatura.DataFim,
                ValorMensal = assinatura.ValorMensal
            } : null
        };
    }
}
