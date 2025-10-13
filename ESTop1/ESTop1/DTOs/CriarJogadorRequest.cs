using ESTop1.Domain;

namespace ESTop1.Api.DTOs;

/// <summary>
/// Request para criar jogador
/// </summary>
public class CriarJogadorRequest
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
