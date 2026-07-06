namespace ESTop1.Domain;

public class Assinatura
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public PlanoAssinatura Plano { get; set; }
    public StatusAssinatura Status { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public decimal ValorMensal { get; set; }
    public string? IdTransacao { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? DataCancelamento { get; set; }
}
