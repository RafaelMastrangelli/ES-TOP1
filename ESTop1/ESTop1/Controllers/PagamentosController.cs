using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ESTop1.Api.Controllers;

[ApiController]
[Route("api/pagamentos")]
public class PagamentosController : ControllerBase
{
    private readonly IPagamentoService _pagamentoService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PagamentosController> _logger;

    public PagamentosController(
        IPagamentoService pagamentoService,
        IConfiguration configuration,
        ILogger<PagamentosController> logger)
    {
        _pagamentoService = pagamentoService;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Inicia checkout de pagamento para um plano
    /// </summary>
    [HttpPost("checkout")]
    [Authorize]
    public async Task<IActionResult> IniciarCheckout([FromBody] IniciarPagamentoRequest request, CancellationToken ct)
    {
        try
        {
            if (!Enum.TryParse<PlanoAssinatura>(request.Plano, true, out var plano))
            {
                return BadRequest(new { message = "Plano inválido" });
            }

            var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)!.Value;
            var metodo = string.IsNullOrWhiteSpace(request.Metodo) ? "checkout" : request.Metodo;

            var resultado = await _pagamentoService.IniciarPagamentoAsync(userId, email, plano, metodo, ct);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao iniciar checkout");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Consulta status de um pagamento
    /// </summary>
    [HttpGet("{id:guid}/status")]
    [Authorize]
    public async Task<IActionResult> ObterStatus(Guid id, CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var status = await _pagamentoService.ObterStatusAsync(id, userId, ct);

        if (status is null)
        {
            return NotFound();
        }

        return Ok(status);
    }

    /// <summary>
    /// Histórico de pagamentos do usuário
    /// </summary>
    [HttpGet("historico")]
    [Authorize]
    public async Task<IActionResult> ListarHistorico(CancellationToken ct)
    {
        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var historico = await _pagamentoService.ListarHistoricoAsync(userId, ct);
        return Ok(historico);
    }

    /// <summary>
    /// Webhook do Mercado Pago
    /// </summary>
    [HttpPost("webhook/mercadopago")]
    [AllowAnonymous]
    public async Task<IActionResult> WebhookMercadoPago([FromQuery] string? topic, [FromQuery] string? id, CancellationToken ct)
    {
        if (!string.Equals(topic, "payment", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(id))
        {
            return Ok();
        }

        try
        {
            await _pagamentoService.ProcessarNotificacaoMercadoPagoAsync(id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar webhook Mercado Pago {PaymentId}", id);
        }

        return Ok();
    }

    /// <summary>
    /// Simula aprovação de pagamento em desenvolvimento (sem Mercado Pago configurado)
    /// </summary>
    [HttpPost("{id:guid}/simular-aprovacao")]
    [Authorize]
    public async Task<IActionResult> SimularAprovacao(Guid id, CancellationToken ct)
    {
        if (!string.Equals(_configuration["ASPNETCORE_ENVIRONMENT"], "Development", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound();
        }

        var userId = Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
        var resultado = await _pagamentoService.SimularAprovacaoDevAsync(id, userId, ct);

        if (resultado is null)
        {
            return NotFound();
        }

        return Ok(resultado);
    }
}

public class IniciarPagamentoRequest
{
    public string Plano { get; set; } = null!;
    public string? Metodo { get; set; }
}
