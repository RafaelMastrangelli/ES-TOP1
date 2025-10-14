using ESTop1.Domain;
using System.ComponentModel.DataAnnotations;

namespace ESTop1.Api.DTOs;

/// <summary>
/// Request para criar jogador
/// </summary>
public class CriarJogadorRequest
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
    
    [Required(ErrorMessage = "Status é obrigatório")]
    public StatusJogador Status { get; set; }
    
    [Required(ErrorMessage = "Disponibilidade é obrigatória")]
    public Disponibilidade Disponibilidade { get; set; }
    
    public Guid? TimeAtualId { get; set; }
    
    [Range(0, 5000000, ErrorMessage = "Valor de mercado deve estar entre 0 e 5.000.000")]
    public decimal ValorDeMercado { get; set; }
}
