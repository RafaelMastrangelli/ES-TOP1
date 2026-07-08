using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ESTop1.Domain;
using ESTop1.Domain.DTOs;
using ESTop1.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ESTop1.Infrastructure.Services;

public class PagamentoService : IPagamentoService
{
    private readonly IPagamentoRepository _pagamentoRepository;
    private readonly IPlanoRepository _planoRepository;
    private readonly IAssinaturaService _assinaturaService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PagamentoService> _logger;

    public PagamentoService(
        IPagamentoRepository pagamentoRepository,
        IPlanoRepository planoRepository,
        IAssinaturaService assinaturaService,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<PagamentoService> logger)
    {
        _pagamentoRepository = pagamentoRepository;
        _planoRepository = planoRepository;
        _assinaturaService = assinaturaService;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    private bool IsDevelopment() =>
        string.Equals(_configuration["ASPNETCORE_ENVIRONMENT"], "Development", StringComparison.OrdinalIgnoreCase);

    public async Task<CheckoutPagamentoResult> IniciarPagamentoAsync(
        Guid usuarioId,
        string email,
        PlanoAssinatura plano,
        string metodo,
        CancellationToken cancellationToken = default)
    {
        var planoDetalhes = await _planoRepository.ObterAtivoPorTipoAsync(plano)
            ?? throw new ArgumentException("Plano não encontrado ou inativo");

        if (plano == PlanoAssinatura.Gratuito)
        {
            var assinatura = await _assinaturaService.CriarAssinaturaAsync(usuarioId, plano);
            var pagamentoGratuito = await _pagamentoRepository.CriarAsync(new Pagamento
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Plano = plano,
                Valor = 0,
                Status = StatusPagamento.Aprovado,
                MetodoPagamento = "gratuito",
                AssinaturaId = assinatura.Id,
                PagoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow
            }, cancellationToken);

            return MapearCheckout(pagamentoGratuito, aprovadoImediatamente: true);
        }

        var pagamento = await _pagamentoRepository.CriarAsync(new Pagamento
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Plano = plano,
            Valor = planoDetalhes.ValorMensal,
            Status = StatusPagamento.Pendente,
            MetodoPagamento = metodo,
            ExpiraEm = DateTime.UtcNow.AddHours(24)
        }, cancellationToken);

        if (!PossuiMercadoPagoConfigurado())
        {
            if (!IsDevelopment())
            {
                throw new InvalidOperationException("Gateway de pagamento não configurado.");
            }

            _logger.LogWarning("Mercado Pago não configurado. Pagamento {PagamentoId} criado em modo desenvolvimento.", pagamento.Id);
            return MapearCheckout(pagamento);
        }

        if (metodo.Equals("pix", StringComparison.OrdinalIgnoreCase))
        {
            await CriarPagamentoPixAsync(pagamento, email, cancellationToken);
        }
        else
        {
            await CriarPreferenciaCheckoutAsync(pagamento, email, planoDetalhes.Nome, cancellationToken);
        }

        await _pagamentoRepository.AtualizarAsync(pagamento, cancellationToken);
        return MapearCheckout(pagamento);
    }

    public async Task<PagamentoStatusResult?> ObterStatusAsync(Guid pagamentoId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var pagamento = await _pagamentoRepository.ObterPorIdAsync(pagamentoId, cancellationToken);
        if (pagamento is null || pagamento.UsuarioId != usuarioId)
        {
            return null;
        }

        if (pagamento.Status == StatusPagamento.Pendente && !string.IsNullOrEmpty(pagamento.IdExterno) && PossuiMercadoPagoConfigurado())
        {
            await SincronizarStatusMercadoPagoAsync(pagamento, cancellationToken);
            await _pagamentoRepository.AtualizarAsync(pagamento, cancellationToken);
        }

        return MapearStatus(pagamento);
    }

    public async Task<List<PagamentoStatusResult>> ListarHistoricoAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var pagamentos = await _pagamentoRepository.ListarPorUsuarioAsync(usuarioId, cancellationToken);
        return pagamentos.Select(MapearStatus).ToList();
    }

    public async Task ProcessarNotificacaoMercadoPagoAsync(string paymentId, CancellationToken cancellationToken = default)
    {
        if (!PossuiMercadoPagoConfigurado())
        {
            return;
        }

        var pagamentoRemoto = await ObterPagamentoMercadoPagoAsync(paymentId, cancellationToken);
        if (pagamentoRemoto is null)
        {
            return;
        }

        var externalReference = pagamentoRemoto.RootElement.GetProperty("external_reference").GetString();
        if (!Guid.TryParse(externalReference, out var pagamentoId))
        {
            pagamentoId = Guid.Empty;
        }

        Pagamento? pagamento = pagamentoId != Guid.Empty
            ? await _pagamentoRepository.ObterPorIdAsync(pagamentoId, cancellationToken)
            : await _pagamentoRepository.ObterPorIdExternoAsync(paymentId, cancellationToken);

        if (pagamento is null)
        {
            _logger.LogWarning("Pagamento não encontrado para notificação MP {PaymentId}", paymentId);
            return;
        }

        pagamento.IdExterno = paymentId;
        await AplicarStatusMercadoPagoAsync(pagamento, pagamentoRemoto.RootElement.GetProperty("status").GetString()!, cancellationToken);
        await _pagamentoRepository.AtualizarAsync(pagamento, cancellationToken);
    }

    public async Task<CheckoutPagamentoResult?> SimularAprovacaoDevAsync(Guid pagamentoId, Guid usuarioId, CancellationToken cancellationToken = default)
    {
        if (!IsDevelopment())
        {
            throw new InvalidOperationException("Simulação disponível apenas em desenvolvimento.");
        }

        var pagamento = await _pagamentoRepository.ObterPorIdAsync(pagamentoId, cancellationToken);
        if (pagamento is null || pagamento.UsuarioId != usuarioId)
        {
            return null;
        }

        if (pagamento.Status != StatusPagamento.Pendente)
        {
            return MapearCheckout(pagamento, pagamento.Status == StatusPagamento.Aprovado);
        }

        await AprovarPagamentoAsync(pagamento, $"dev-{Guid.NewGuid()}", cancellationToken);
        await _pagamentoRepository.AtualizarAsync(pagamento, cancellationToken);
        return MapearCheckout(pagamento, aprovadoImediatamente: true);
    }

    private async Task CriarPreferenciaCheckoutAsync(Pagamento pagamento, string email, string nomePlano, CancellationToken cancellationToken)
    {
        var client = CriarClienteMercadoPago();
        var payload = new
        {
            items = new[]
            {
                new
                {
                    title = $"ESTop1 - {nomePlano}",
                    quantity = 1,
                    unit_price = pagamento.Valor,
                    currency_id = "BRL"
                }
            },
            payer = new { email },
            external_reference = pagamento.Id.ToString(),
            notification_url = _configuration["MercadoPago:NotificationUrl"],
            back_urls = new
            {
                success = $"{_configuration["MercadoPago:SuccessUrl"]}?pagamentoId={pagamento.Id}",
                failure = _configuration["MercadoPago:FailureUrl"],
                pending = $"{_configuration["MercadoPago:PendingUrl"]}?pagamentoId={pagamento.Id}&pending=1"
            },
            auto_return = "approved"
        };

        var response = await client.PostAsJsonAsync("checkout/preferences", payload, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Erro ao criar preferência MP: {Body}", body);
            throw new InvalidOperationException("Não foi possível iniciar o checkout de pagamento.");
        }

        using var document = JsonDocument.Parse(body);
        var usarSandbox = _configuration.GetValue("MercadoPago:UsarSandbox", true);
        pagamento.CheckoutUrl = usarSandbox
            ? document.RootElement.GetProperty("sandbox_init_point").GetString()
            : document.RootElement.GetProperty("init_point").GetString();
        pagamento.IdExterno = document.RootElement.GetProperty("id").GetString();
        pagamento.MetodoPagamento = "checkout";
    }

    private async Task CriarPagamentoPixAsync(Pagamento pagamento, string email, CancellationToken cancellationToken)
    {
        var client = CriarClienteMercadoPago();
        var payload = new
        {
            transaction_amount = pagamento.Valor,
            payment_method_id = "pix",
            external_reference = pagamento.Id.ToString(),
            payer = new { email },
            notification_url = _configuration["MercadoPago:NotificationUrl"]
        };

        var response = await client.PostAsJsonAsync("v1/payments", payload, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Erro ao criar PIX MP: {Body}", body);
            throw new InvalidOperationException("Não foi possível gerar o pagamento PIX.");
        }

        using var document = JsonDocument.Parse(body);
        pagamento.IdExterno = document.RootElement.GetProperty("id").GetString();
        pagamento.MetodoPagamento = "pix";

        if (document.RootElement.TryGetProperty("point_of_interaction", out var poi) &&
            poi.TryGetProperty("transaction_data", out var tx))
        {
            pagamento.PixQrCode = tx.TryGetProperty("qr_code", out var qr) ? qr.GetString() : null;
            pagamento.PixQrCodeBase64 = tx.TryGetProperty("qr_code_base64", out var qr64) ? qr64.GetString() : null;
        }
    }

    private async Task SincronizarStatusMercadoPagoAsync(Pagamento pagamento, CancellationToken cancellationToken)
    {
        var remoto = await ObterPagamentoMercadoPagoAsync(pagamento.IdExterno!, cancellationToken);
        if (remoto is null) return;

        var status = remoto.RootElement.GetProperty("status").GetString()!;
        await AplicarStatusMercadoPagoAsync(pagamento, status, cancellationToken);
    }

    private async Task AplicarStatusMercadoPagoAsync(Pagamento pagamento, string statusMercadoPago, CancellationToken cancellationToken)
    {
        switch (statusMercadoPago)
        {
            case "approved":
                if (pagamento.Status != StatusPagamento.Aprovado)
                {
                    await AprovarPagamentoAsync(pagamento, pagamento.IdExterno ?? pagamento.Id.ToString(), cancellationToken);
                }
                break;
            case "rejected":
            case "cancelled":
                pagamento.Status = StatusPagamento.Recusado;
                break;
            case "pending":
            case "in_process":
                pagamento.Status = StatusPagamento.Pendente;
                break;
        }
    }

    private async Task AprovarPagamentoAsync(Pagamento pagamento, string idTransacao, CancellationToken cancellationToken)
    {
        pagamento.Status = StatusPagamento.Aprovado;
        pagamento.PagoEm = DateTime.UtcNow;

        if (pagamento.AssinaturaId is null)
        {
            var assinatura = await _assinaturaService.AtivarAssinaturaPorPagamentoAsync(
                pagamento.UsuarioId,
                pagamento.Plano,
                idTransacao);
            pagamento.AssinaturaId = assinatura.Id;
        }
    }

    private async Task<JsonDocument?> ObterPagamentoMercadoPagoAsync(string paymentId, CancellationToken cancellationToken)
    {
        var client = CriarClienteMercadoPago();
        var response = await client.GetAsync($"v1/payments/{paymentId}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Falha ao consultar pagamento MP {PaymentId}", paymentId);
            return null;
        }

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonDocument.Parse(body);
    }

    private HttpClient CriarClienteMercadoPago()
    {
        var client = _httpClientFactory.CreateClient("MercadoPago");
        var token = ObterAccessToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private bool PossuiMercadoPagoConfigurado()
    {
        return !string.IsNullOrWhiteSpace(ObterAccessToken());
    }

    private string? ObterAccessToken()
    {
        return _configuration["MercadoPago:AccessToken"]
            ?? Environment.GetEnvironmentVariable("MERCADOPAGO_ACCESS_TOKEN");
    }

    private static CheckoutPagamentoResult MapearCheckout(Pagamento pagamento, bool aprovadoImediatamente = false)
    {
        return new CheckoutPagamentoResult
        {
            PagamentoId = pagamento.Id,
            Status = pagamento.Status.ToString(),
            Plano = pagamento.Plano.ToString(),
            Valor = pagamento.Valor,
            MetodoPagamento = pagamento.MetodoPagamento,
            CheckoutUrl = pagamento.CheckoutUrl,
            PixQrCode = pagamento.PixQrCode,
            PixQrCodeBase64 = pagamento.PixQrCodeBase64,
            ExpiraEm = pagamento.ExpiraEm,
            AprovadoImediatamente = aprovadoImediatamente,
            AssinaturaId = pagamento.AssinaturaId
        };
    }

    private static PagamentoStatusResult MapearStatus(Pagamento pagamento)
    {
        return new PagamentoStatusResult
        {
            PagamentoId = pagamento.Id,
            Status = pagamento.Status.ToString(),
            Plano = pagamento.Plano.ToString(),
            Valor = pagamento.Valor,
            MetodoPagamento = pagamento.MetodoPagamento,
            PagoEm = pagamento.PagoEm,
            AssinaturaId = pagamento.AssinaturaId
        };
    }
}
