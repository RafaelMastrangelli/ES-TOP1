namespace ESTop1.Domain;

public class Estatistica
{
    public Guid Id { get; set; }
    public Guid JogadorId { get; set; }
    public Jogador Jogador { get; set; } = null!;
    public string Periodo { get; set; } = "Geral";
    public decimal Rating { get; set; }
    public decimal KD { get; set; }
    public int PartidasJogadas { get; set; }
}
