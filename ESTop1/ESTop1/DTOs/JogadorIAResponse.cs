using System.Text.Json.Serialization;

namespace ESTop1.DTOs;

public class JogadorIAResponse
{
    [JsonPropertyName("apelido")]
    public string Apelido { get; set; } = null!;

    [JsonPropertyName("pais")]
    public string Pais { get; set; } = "BR";

    [JsonPropertyName("idade")]
    public int Idade { get; set; }

    [JsonPropertyName("timeAtual")]
    public string? TimeAtual { get; set; }

    [JsonPropertyName("funcaoPrincipal")]
    public string FuncaoPrincipal { get; set; } = null!;

    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [JsonPropertyName("rating")]
    public decimal Rating { get; set; }

    [JsonPropertyName("valorDeMercado")]
    public decimal ValorDeMercado { get; set; }

    [JsonPropertyName("fotoUrl")]
    public string? FotoUrl { get; set; }
}
