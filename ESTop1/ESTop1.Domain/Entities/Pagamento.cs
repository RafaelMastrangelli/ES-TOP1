namespace ESTop1.Domain;

public class Pagamento
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public PlanoAssinatura Plano { get; set; }
    public decimal Valor { get; set; }
    public StatusPagamento Status { get; set; } = StatusPagamento.Pendente;
    public string MetodoPagamento { get; set; } = null!;
    public string? IdExterno { get; set; }
    public string? CheckoutUrl { get; set; }
    public string? PixQrCode { get; set; }
    public string? PixQrCodeBase64 { get; set; }
    public Guid? AssinaturaId { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? PagoEm { get; set; }
    public DateTime ExpiraEm { get; set; }
}
