namespace ESTop1.Domain;

public class Time
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Pais { get; set; } = "BR";
    public int? Tier { get; set; }
    public bool? Contratando { get; set; }
    public string? LogoUrl { get; set; }
    public List<Jogador> Jogadores { get; set; } = new();
}
