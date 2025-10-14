using ESTop1.Api.DTOs;
using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESTop1.Api.Controllers;

[ApiController]
[Route("api/assinaturas")]
[Authorize]
public class AssinaturasController : ControllerBase
{
    private readonly IAssinaturaService _assinaturaService;

    public AssinaturasController(IAssinaturaService assinaturaService)
    {
        _assinaturaService = assinaturaService;
    }

    /// <summary>
    /// Obtém planos disponíveis
    /// </summary>
    [HttpGet("planos")]
    public async Task<IActionResult> ObterPlanos()
    {
        try
        {
            var planos = await _assinaturaService.ObterPlanosDisponiveisAsync();
            var response = planos.Select(p => new PlanoResponse
            {
                Id = p.Id,
                Tipo = p.Tipo.ToString(),
                Nome = p.Nome,
                Descricao = p.Descricao,
                ValorMensal = p.ValorMensal,
                LimiteJogadores = p.LimiteJogadores,
                AcessoEstatisticas = p.AcessoEstatisticas,
                AcessoBuscaIA = p.AcessoBuscaIA,
                AcessoAPI = p.AcessoAPI,
                SuportePrioritario = p.SuportePrioritario
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao obter planos: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém assinatura ativa do usuário
    /// </summary>
    [HttpGet("minha")]
    public async Task<IActionResult> ObterMinhaAssinatura()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var assinatura = await _assinaturaService.ObterAssinaturaAtivaAsync(userId);

            if (assinatura == null)
                return NotFound("Nenhuma assinatura ativa encontrada");

            var response = new AssinaturaResponse
            {
                Id = assinatura.Id,
                Plano = assinatura.Plano.ToString(),
                Status = assinatura.Status.ToString(),
                DataInicio = assinatura.DataInicio,
                DataFim = assinatura.DataFim,
                ValorMensal = assinatura.ValorMensal
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao obter assinatura: {ex.Message}");
        }
    }

    /// <summary>
    /// Cria nova assinatura
    /// </summary>
    [HttpPost("criar")]
    public async Task<IActionResult> CriarAssinatura([FromBody] CriarAssinaturaRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var assinatura = await _assinaturaService.CriarAssinaturaAsync(userId, request.Plano);

            var response = new AssinaturaResponse
            {
                Id = assinatura.Id,
                Plano = assinatura.Plano.ToString(),
                Status = assinatura.Status.ToString(),
                DataInicio = assinatura.DataInicio,
                DataFim = assinatura.DataFim,
                ValorMensal = assinatura.ValorMensal
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar assinatura: {ex.Message}");
        }
    }

    /// <summary>
    /// Cancela assinatura
    /// </summary>
    [HttpPost("{id}/cancelar")]
    public async Task<IActionResult> CancelarAssinatura(Guid id)
    {
        try
        {
            await _assinaturaService.CancelarAssinaturaAsync(id);
            return Ok("Assinatura cancelada com sucesso");
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao cancelar assinatura: {ex.Message}");
        }
    }

    /// <summary>
    /// Renova assinatura
    /// </summary>
    [HttpPost("{id}/renovar")]
    public async Task<IActionResult> RenovarAssinatura(Guid id)
    {
        try
        {
            await _assinaturaService.RenovarAssinaturaAsync(id);
            return Ok("Assinatura renovada com sucesso");
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao renovar assinatura: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza assinatura de usuário por email (endpoint administrativo)
    /// </summary>
    [HttpPost("admin/atualizar-por-email")]
    [AllowAnonymous]
    public async Task<IActionResult> AtualizarAssinaturaPorEmail([FromBody] AtualizarAssinaturaPorEmailRequest request)
    {
        try
        {
            var assinatura = await _assinaturaService.AtualizarAssinaturaPorEmailAsync(request.Email, request.Plano);
            
            var response = new AssinaturaResponse
            {
                Id = assinatura.Id,
                Plano = assinatura.Plano.ToString(),
                Status = assinatura.Status.ToString(),
                DataInicio = assinatura.DataInicio,
                DataFim = assinatura.DataFim,
                ValorMensal = assinatura.ValorMensal
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao atualizar assinatura: {ex.Message}");
        }
    }
}

public class CriarAssinaturaRequest
{
    public PlanoAssinatura Plano { get; set; }
}

public class AtualizarAssinaturaPorEmailRequest
{
    public string Email { get; set; } = null!;
    public PlanoAssinatura Plano { get; set; }
}
