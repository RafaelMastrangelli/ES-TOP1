namespace ESTop1.Api.DTOs;

/// <summary>
/// Request para criar time
/// </summary>
public class CriarTimeRequest
{
    public string Nome { get; set; } = null!;
    public string? Pais { get; set; }
}
