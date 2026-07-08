namespace ESTop1.Domain.DTOs;

public class CheckoutPagamentoResult
{
    public Guid PagamentoId { get; set; }
    public string Status { get; set; } = null!;
    public string Plano { get; set; } = null!;
    public decimal Valor { get; set; }
    public string MetodoPagamento { get; set; } = null!;
    public string? CheckoutUrl { get; set; }
    public string? PixQrCode { get; set; }
    public string? PixQrCodeBase64 { get; set; }
    public DateTime ExpiraEm { get; set; }
    public bool AprovadoImediatamente { get; set; }
    public Guid? AssinaturaId { get; set; }
}

public class PagamentoStatusResult
{
    public Guid PagamentoId { get; set; }
    public string Status { get; set; } = null!;
    public string Plano { get; set; } = null!;
    public decimal Valor { get; set; }
    public string MetodoPagamento { get; set; } = null!;
    public DateTime? PagoEm { get; set; }
    public Guid? AssinaturaId { get; set; }
}
