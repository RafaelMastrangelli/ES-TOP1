using System.ComponentModel.DataAnnotations;

namespace ESTop1.Api.DTOs;

/// <summary>
/// Request para criar time
/// </summary>
public class CriarTimeRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    public string Nome { get; set; } = null!;
    
    [StringLength(2, ErrorMessage = "País deve ter 2 caracteres")]
    public string? Pais { get; set; }
}
