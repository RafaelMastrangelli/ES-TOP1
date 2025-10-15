using ESTop1.Api.Attributes;
using ESTop1.Api.DTOs;
using ESTop1.Api.Middleware;
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
    
    public JogadoresController(IJogadorService jogadorService)
    {
        _jogadorService = jogadorService;
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
            var (jogadores, total) = await _jogadorService.ListarJogadoresAsync(filtro, ct);
            return Ok(new { total, page = filtro.Page, pageSize = filtro.PageSize, items = jogadores });
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
            var resultado = await _jogadorService.CriarJogadorAsync(request, ct);
            return CreatedAtAction(nameof(Obter), new { id = ((dynamic)resultado).Id }, resultado);
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

            // Log para debug
            Console.WriteLine($"Buscando jogador para usuário ID: {userIdGuid}");

            var jogador = await _jogadorService.ObterJogadorPorUsuarioIdAsync(userIdGuid, ct);
            
            if (jogador == null)
            {
                Console.WriteLine($"Jogador não encontrado para usuário ID: {userIdGuid}");
                return NotFound("Perfil de jogador não encontrado");
            }

            Console.WriteLine($"Jogador encontrado: {jogador}");
            return Ok(jogador);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao obter perfil: {ex.Message}");
            return BadRequest($"Erro ao obter perfil: {ex.Message}");
        }
    }

    /// <summary>
    /// DEBUG: Lista todos os jogadores (temporário)
    /// </summary>
    [HttpGet("debug/todos")]
    public async Task<IActionResult> DebugTodosJogadores(CancellationToken ct)
    {
        try
        {
            var jogadores = await _jogadorService.ListarJogadoresAsync(new { }, ct);
            return Ok(new { 
                total = jogadores.total, 
                jogadores = jogadores.jogadores,
                message = "Lista de todos os jogadores para debug"
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao listar jogadores: {ex.Message}");
        }
    }

    /// <summary>
    /// DEBUG: Criar jogador para usuário atual (temporário)
    /// </summary>
    [HttpPost("debug/criar-para-usuario")]
    [AuthorizeJogador]
    public async Task<IActionResult> DebugCriarJogadorParaUsuario(CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized();

            var nome = User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuário";
            
            var jogador = await _jogadorService.CriarJogadorParaUsuarioAsync(userIdGuid, nome, ct);
            
            return Ok(new { 
                message = "Jogador criado com sucesso",
                jogador = jogador
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar jogador: {ex.Message}");
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

