namespace ESTop1.Domain.DTOs;

public class CriarInscricaoCommand
{
    public string Apelido { get; set; } = null!;
    public string? Pais { get; set; }
    public int Idade { get; set; }
    public Funcao FuncaoPrincipal { get; set; }
    public decimal? Rating { get; set; }
    public decimal? KD { get; set; }
    public int? PartidasJogadas { get; set; }
}

public class InscricaoCriadaDto
{
    public Guid InscricaoId { get; set; }
    public string Message { get; set; } = null!;
}

public class InscricaoMensagemDto
{
    public string Message { get; set; } = null!;
}
