namespace ESTop1.Domain;

public class Jogador
{
    public Guid Id { get; set; }
    public string Apelido { get; set; } = null!;
    public string Pais { get; set; } = "BR";
    public int Idade { get; set; }
    public Funcao FuncaoPrincipal { get; set; }
    public StatusJogador Status { get; set; }
    public Disponibilidade Disponibilidade { get; set; }
    public Guid? TimeAtualId { get; set; }
    public Time? TimeAtual { get; set; }
    public decimal ValorDeMercado { get; set; }
    public string? FotoUrl { get; set; }
    public bool Visivel { get; set; } = true;
    public List<Estatistica> Estatisticas { get; set; } = new();
}
