using ESTop1.Domain;

namespace ESTop1.Api.DTOs;

/// <summary>
/// Filtros para listagem de jogadores
/// </summary>
public class FiltroJogador
{
    public string? Q { get; set; }
    public string? Pais { get; set; }
    public StatusJogador? Status { get; set; }
    public Disponibilidade? Disp { get; set; }
    public Funcao? Funcao { get; set; }
    public decimal? MinRating { get; set; }
    public int? MaxIdade { get; set; }
    public string? Ordenar { get; set; } = "apelido_asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
