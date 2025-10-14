namespace ESTop1.Domain;

public enum StatusJogador { Profissional, Aposentado, Amador }
public enum Disponibilidade { EmTime, Livre, Teste }
public enum Funcao { Entry, Suporte, Awp, Igl, Lurker }

public class Time
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Pais { get; set; } = "BR";
    public List<Jogador> Jogadores { get; set; } = new();
}

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
