using ESTop1.Domain;
using System.ComponentModel.DataAnnotations;

namespace ESTop1.Api.DTOs;

public class RegistroRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    public string Nome { get; set; } = null!;
    
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = null!;
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
    public string Senha { get; set; } = null!;
    
    [Required(ErrorMessage = "Tipo é obrigatório")]
    public string Tipo { get; set; } = null!;
    
    public TipoUsuario GetTipoUsuario()
    {
        return Enum.TryParse<TipoUsuario>(Tipo, true, out var tipo) 
            ? tipo 
            : TipoUsuario.Jogador; // Default para Jogador
    }
    
    /// <summary>
    /// Valida se o tipo de usuário é válido
    /// </summary>
    public bool IsValidTipo()
    {
        return Enum.TryParse<TipoUsuario>(Tipo, true, out _);
    }
    
    /// <summary>
    /// Obtém os tipos de usuário disponíveis para registro
    /// </summary>
    public static string[] GetTiposDisponiveis()
    {
        return new[] { "Jogador", "Organizacao" }; // Admin não pode ser registrado via API
    }
}
