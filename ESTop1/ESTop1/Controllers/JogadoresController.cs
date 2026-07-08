using ESTop1.Api.Attributes;
using ESTop1.Api.DTOs;
using ESTop1.Domain.DTOs;
using ESTop1.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ESTop1.Api.Controllers;

/// <summary>
/// Controller para gerenciar jogadores profissionais de CS2
/// </summary>
[ApiController]
[Route("api/jogadores")]
public class JogadoresController : ControllerBase
{
    private readonly IJogadorService _jogadorService;
    private readonly ILogger<JogadoresController> _logger;
    
    public JogadoresController(IJogadorService jogadorService, ILogger<JogadoresController> logger)
    {
        _jogadorService = jogadorService;
        _logger = logger;
    }

    /// <summary>
    /// Lista jogadores com filtros e paginação
    /// </summary>
    [HttpGet]
    [Authorize]
    [RequerAssinatura("buscar_jogadores")]
    public async Task<IActionResult> Listar([FromQuery] FiltroJogador filtro, CancellationToken ct)
    {
        try
        {
            var resultado = await _jogadorService.ListarJogadoresAsync(filtro, ct);
            return Ok(new { total = resultado.Total, page = filtro.Page, pageSize = filtro.PageSize, items = resultado.Items });
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao listar jogadores: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém detalhes de um jogador específico
    /// </summary>
    [HttpGet("{id}")]
    [Authorize]
    [RequerAssinatura("estatisticas")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        try
        {
            var jogador = await _jogadorService.ObterJogadorPorIdAsync(id, ct);
            
            if (jogador == null)
                return NotFound();

            return Ok(jogador);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao obter jogador: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza as fotos dos jogadores existentes com URLs personalizadas
    /// </summary>
    [HttpPost("atualizar-fotos")]
    [AuthorizeAdmin]
    public async Task<IActionResult> AtualizarFotos(CancellationToken ct)
    {
        try
        {
            var resultado = await _jogadorService.AtualizarFotosJogadoresAsync(ct);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao atualizar fotos: {ex.Message}");
        }
    }

    /// <summary>
    /// Cria um novo jogador
    /// </summary>
    [HttpPost]
    [AuthorizeOrganizacao]
    [RequerAssinatura("gerenciar_jogadores")]
    public async Task<IActionResult> Criar([FromBody] CriarJogadorRequest request, CancellationToken ct)
    {
        try
        {
            var resultado = await _jogadorService.CriarJogadorAsync(new CriarJogadorCommand
            {
                Apelido = request.Apelido,
                Pais = request.Pais,
                Idade = request.Idade,
                FuncaoPrincipal = request.FuncaoPrincipal,
                Status = request.Status,
                Disponibilidade = request.Disponibilidade,
                TimeAtualId = request.TimeAtualId,
                ValorDeMercado = request.ValorDeMercado
            }, ct);
            return CreatedAtAction(nameof(Obter), new { id = resultado.Id }, resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar jogador: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém o perfil do jogador logado
    /// </summary>
    [HttpGet("meu-perfil")]
    [AuthorizeJogador]
    public async Task<IActionResult> MeuPerfil(CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized();

            var jogador = await _jogadorService.ObterJogadorPorUsuarioIdAsync(userIdGuid, ct);
            
            if (jogador == null)
            {
                _logger.LogWarning("Perfil de jogador não encontrado para usuário {UsuarioId}", userIdGuid);
                return NotFound("Perfil de jogador não encontrado");
            }

            return Ok(jogador);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter perfil do jogador");
            return BadRequest($"Erro ao obter perfil: {ex.Message}");
        }
    }

    /// <summary>
    /// Cria o perfil de jogador para o usuário logado
    /// </summary>
    [HttpPost("meu-perfil")]
    [AuthorizeJogador]
    public async Task<IActionResult> CriarMeuPerfil(CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized();

            var existente = await _jogadorService.ObterJogadorPorUsuarioIdAsync(userIdGuid, ct);
            if (existente != null)
                return Conflict("Perfil de jogador já existe");

            var nome = User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuário";
            var jogador = await _jogadorService.CriarJogadorParaUsuarioAsync(userIdGuid, nome, ct);

            return CreatedAtAction(nameof(MeuPerfil), jogador);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar perfil do jogador");
            return BadRequest($"Erro ao criar perfil: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza o perfil do jogador logado
    /// </summary>
    [HttpPut("meu-perfil")]
    [AuthorizeJogador]
    public async Task<IActionResult> AtualizarMeuPerfil([FromBody] AtualizarJogadorRequest dados, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized();

            var resultado = await _jogadorService.AtualizarJogadorAsync(userIdGuid, new AtualizarJogadorCommand
            {
                Apelido = dados.Apelido,
                Pais = dados.Pais,
                Idade = dados.Idade,
                FuncaoPrincipal = dados.FuncaoPrincipal,
                Status = dados.Status,
                Disponibilidade = dados.Disponibilidade,
                ValorDeMercado = dados.ValorDeMercado,
                FotoUrl = dados.FotoUrl
            }, ct);
            
            if (resultado == null)
                return NotFound("Perfil de jogador não encontrado");

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao atualizar perfil: {ex.Message}");
        }
    }

    /// <summary>
    /// Altera a visibilidade de um jogador
    /// </summary>
    [HttpPut("{id}/visibilidade")]
    public async Task<IActionResult> AlterarVisibilidade(Guid id, [FromQuery] bool on, CancellationToken ct)
    {
        try
        {
            var sucesso = await _jogadorService.AlterarVisibilidadeJogadorAsync(id, on, ct);
            
            if (!sucesso)
                return NotFound();

            return Ok(new { message = $"Jogador {(on ? "tornado visível" : "ocultado")} com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao alterar visibilidade: {ex.Message}");
        }
    }
}

