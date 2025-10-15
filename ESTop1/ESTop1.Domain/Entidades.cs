namespace ESTop1.Domain;

public enum StatusJogador { Profissional, Aposentado, Amador }
public enum Disponibilidade { Contratado, Livre }
public enum Funcao { Entry, Suporte, Awp, Igl, Lurker }
public enum TipoUsuario 
{ 
    Admin,           // Administrador do sistema
    Organizacao,     // Organização/Time (pode gerenciar jogadores)
    Jogador          // Jogador individual
}
public enum StatusAssinatura { Ativa, Inativa, Expirada, Cancelada }
public enum PlanoAssinatura { Gratuito, Mensal, Trimestral, Enterprise }

public class Time
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Pais { get; set; } = "BR";
    public int? Tier { get; set; }
    public bool? Contratando { get; set; }
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

public class Plano
{
    public Guid Id { get; set; }
    public PlanoAssinatura Tipo { get; set; }
    public string Nome { get; set; } = null!;
    public string Descricao { get; set; } = null!;
    public decimal ValorMensal { get; set; }
    public int LimiteJogadores { get; set; }
    public bool AcessoEstatisticas { get; set; }
    public bool AcessoBuscaIA { get; set; }
    public bool AcessoAPI { get; set; }
    public bool SuportePrioritario { get; set; }
    public bool Ativo { get; set; } = true;
}
