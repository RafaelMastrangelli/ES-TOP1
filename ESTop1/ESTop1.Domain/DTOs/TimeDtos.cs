namespace ESTop1.Domain.DTOs;

public class CriarTimeCommand
{
    public string Nome { get; set; } = null!;
    public string? Pais { get; set; }
}

public class AtualizarTimeCommand
{
    public string? Nome { get; set; }
    public string? Pais { get; set; }
    public int? Tier { get; set; }
    public bool? Contratando { get; set; }
    public string? LogoUrl { get; set; }
}

public class JogadorNoTimeDto
{
    public Guid Id { get; set; }
    public string Apelido { get; set; } = null!;
    public string Pais { get; set; } = null!;
    public int Idade { get; set; }
    public Funcao FuncaoPrincipal { get; set; }
    public StatusJogador Status { get; set; }
    public Disponibilidade Disponibilidade { get; set; }
    public decimal ValorDeMercado { get; set; }
    public string? FotoUrl { get; set; }
}

public class TimeListagemDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Pais { get; set; } = null!;
    public int? Tier { get; set; }
    public bool? Contratando { get; set; }
    public int QuantidadeJogadores { get; set; }
}

public class TimesPaginadosDto
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IReadOnlyList<TimeListagemDto> Items { get; set; } = Array.Empty<TimeListagemDto>();
}

public class TimeDetalheDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Pais { get; set; } = null!;
    public int? Tier { get; set; }
    public bool? Contratando { get; set; }
    public string? LogoUrl { get; set; }
    public IReadOnlyList<JogadorNoTimeDto> Jogadores { get; set; } = Array.Empty<JogadorNoTimeDto>();
}
