using ESTop1.Domain;
using System.ComponentModel.DataAnnotations;

namespace ESTop1.Api.DTOs;

/// <summary>
/// Request para atualizar dados do jogador
/// </summary>
public class AtualizarJogadorRequest
{
    [StringLength(50, ErrorMessage = "Apelido deve ter no máximo 50 caracteres")]
    public string? Apelido { get; set; }
    
    [StringLength(2, ErrorMessage = "País deve ter no máximo 2 caracteres")]
    public string? Pais { get; set; }
    
    [Range(16, 50, ErrorMessage = "Idade deve estar entre 16 e 50 anos")]
    public int? Idade { get; set; }
    
    public Funcao? FuncaoPrincipal { get; set; }
    
    public StatusJogador? Status { get; set; }
    
    public Disponibilidade? Disponibilidade { get; set; }
    
    [Range(0, 5000000, ErrorMessage = "Valor de mercado deve estar entre 0 e 5.000.000")]
    public decimal? ValorDeMercado { get; set; }
    
    public string? FotoUrl { get; set; }
}
