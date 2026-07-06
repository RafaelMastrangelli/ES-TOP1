namespace ESTop1.Domain;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string SenhaHash { get; set; } = null!;
    public TipoUsuario Tipo { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime? UltimoLogin { get; set; }
    public bool Ativo { get; set; } = true;
    public Guid? TimeId { get; set; }
    public Time? Time { get; set; }
    public List<Assinatura> Assinaturas { get; set; } = new();
}
