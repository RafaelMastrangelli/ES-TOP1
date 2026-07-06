namespace ESTop1.Domain.DTOs;

public class CriarJogadorCommand
{
    public string Apelido { get; set; } = null!;
    public string? Pais { get; set; }
    public int Idade { get; set; }
    public Funcao FuncaoPrincipal { get; set; }
    public StatusJogador Status { get; set; }
    public Disponibilidade Disponibilidade { get; set; }
    public Guid? TimeAtualId { get; set; }
    public decimal ValorDeMercado { get; set; }
}

public class AtualizarJogadorCommand
{
    public string? Apelido { get; set; }
    public string? Pais { get; set; }
    public int? Idade { get; set; }
    public Funcao? FuncaoPrincipal { get; set; }
    public StatusJogador? Status { get; set; }
    public Disponibilidade? Disponibilidade { get; set; }
    public decimal? ValorDeMercado { get; set; }
    public string? FotoUrl { get; set; }
}

public class JogadorListagemDto
{
    public Guid Id { get; set; }
    public string Apelido { get; set; } = null!;
    public string Pais { get; set; } = null!;
    public int Idade { get; set; }
    public string? Time { get; set; }
    public Funcao FuncaoPrincipal { get; set; }
    public StatusJogador Status { get; set; }
    public Disponibilidade Disponibilidade { get; set; }
    public decimal ValorDeMercado { get; set; }
    public string? FotoUrl { get; set; }
    public decimal RatingGeral { get; set; }
}

public class JogadoresPaginadosDto
{
    public IReadOnlyList<JogadorListagemDto> Items { get; set; } = Array.Empty<JogadorListagemDto>();
    public int Total { get; set; }
}

public class TimeResumoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Pais { get; set; } = null!;
    public int? Tier { get; set; }
    public bool? Contratando { get; set; }
}

public class EstatisticaDto
{
    public Guid Id { get; set; }
    public string Periodo { get; set; } = null!;
    public decimal Rating { get; set; }
    public decimal KD { get; set; }
    public int PartidasJogadas { get; set; }
}

public class JogadorDetalheDto
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
    public bool Visivel { get; set; }
    public TimeResumoDto? TimeAtual { get; set; }
    public IReadOnlyList<EstatisticaDto> Estatisticas { get; set; } = Array.Empty<EstatisticaDto>();
}

public class JogadorResumoDto
{
    public Guid Id { get; set; }
    public string Apelido { get; set; } = null!;
    public string Pais { get; set; } = null!;
    public int Idade { get; set; }
    public Funcao FuncaoPrincipal { get; set; }
    public StatusJogador Status { get; set; }
    public Disponibilidade Disponibilidade { get; set; }
    public decimal ValorDeMercado { get; set; }
    public bool Visivel { get; set; }
    public TimeResumoDto? TimeAtual { get; set; }
}

public class AtualizarFotosResultDto
{
    public string Message { get; set; } = null!;
    public int TotalAtualizado { get; set; }
}
