using System.ComponentModel.DataAnnotations;

namespace ESTop1.Api.DTOs;

/// <summary>
/// Request para atualizar dados do time
/// </summary>
public class AtualizarTimeRequest
{
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    public string? Nome { get; set; }
    
    [StringLength(2, ErrorMessage = "País deve ter 2 caracteres")]
    public string? Pais { get; set; }
    
    [Range(1, 5, ErrorMessage = "Tier deve estar entre 1 e 5")]
    public int? Tier { get; set; }
    
    public bool? Contratando { get; set; }
    
    [Url(ErrorMessage = "URL da logo deve ser válida")]
    public string? LogoUrl { get; set; }
}
