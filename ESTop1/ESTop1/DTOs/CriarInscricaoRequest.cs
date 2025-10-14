using ESTop1.Domain;
using System.ComponentModel.DataAnnotations;

namespace ESTop1.Api.DTOs;

/// <summary>
/// Request para criar inscrição
/// </summary>
public class CriarInscricaoRequest
{
    [Required(ErrorMessage = "Apelido é obrigatório")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Apelido deve ter entre 2 e 50 caracteres")]
    public string Apelido { get; set; } = null!;
    
    [StringLength(2, ErrorMessage = "País deve ter 2 caracteres")]
    public string? Pais { get; set; }
    
    [Range(16, 50, ErrorMessage = "Idade deve estar entre 16 e 50 anos")]
    public int Idade { get; set; }
    
    [Required(ErrorMessage = "Função principal é obrigatória")]
    public Funcao FuncaoPrincipal { get; set; }
    
    [Range(0.5, 2.0, ErrorMessage = "Rating deve estar entre 0.5 e 2.0")]
    public decimal? Rating { get; set; }
    
    [Range(0.1, 3.0, ErrorMessage = "KD deve estar entre 0.1 e 3.0")]
    public decimal? KD { get; set; }
    
    [Range(1, 10000, ErrorMessage = "Partidas jogadas deve estar entre 1 e 10.000")]
    public int? PartidasJogadas { get; set; }
}
