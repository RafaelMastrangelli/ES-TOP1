namespace ESTop1.Api.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public UsuarioResponse Usuario { get; set; } = null!;
    public AssinaturaResponse? Assinatura { get; set; }
}

public class UsuarioResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Tipo { get; set; } = null!;
    public DateTime DataCriacao { get; set; }
    public DateTime? UltimoLogin { get; set; }
}

public class AssinaturaResponse
{
    public Guid Id { get; set; }
    public string Plano { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public decimal ValorMensal { get; set; }
}
