using ESTop1.Domain;

namespace ESTop1.Api.DTOs;

/// <summary>
/// Request para criar inscrição
/// </summary>
public class CriarInscricaoRequest
{
    public string Apelido { get; set; } = null!;
    public string? Pais { get; set; }
    public int Idade { get; set; }
    public Funcao FuncaoPrincipal { get; set; }
    public decimal? Rating { get; set; }
    public decimal? KD { get; set; }
    public int? PartidasJogadas { get; set; }
}
