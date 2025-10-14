using System.ComponentModel.DataAnnotations;

namespace ESTop1.Domain;

/// <summary>
/// Filtros para listagem de jogadores
/// </summary>
public class FiltroJogador
{
    [StringLength(100, ErrorMessage = "Consulta deve ter no máximo 100 caracteres")]
    public string? Q { get; set; }
    
    [StringLength(2, ErrorMessage = "País deve ter 2 caracteres")]
    public string? Pais { get; set; }
    
    public StatusJogador? Status { get; set; }
    public Disponibilidade? Disp { get; set; }
    public Funcao? Funcao { get; set; }
    
    [Range(0.5, 2.0, ErrorMessage = "Rating mínimo deve estar entre 0.5 e 2.0")]
    public decimal? MinRating { get; set; }
    
    [Range(16, 50, ErrorMessage = "Idade máxima deve estar entre 16 e 50 anos")]
    public int? MaxIdade { get; set; }
    
    public string? Ordenar { get; set; } = "apelido_asc";
    
    [Range(1, int.MaxValue, ErrorMessage = "Página deve ser maior que 0")]
    public int Page { get; set; } = 1;
    
    [Range(1, 100, ErrorMessage = "Tamanho da página deve estar entre 1 e 100")]
    public int PageSize { get; set; } = 20;
}
