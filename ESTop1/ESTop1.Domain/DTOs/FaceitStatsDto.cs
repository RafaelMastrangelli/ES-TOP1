using System.Text.Json.Serialization;

namespace ESTop1.Domain.DTOs;

public class FaceitStatsDto
{
    [JsonPropertyName("lifetime")]
    public FaceitLifetimeDto? Lifetime { get; set; }
}

public class FaceitLifetimeDto
{
    [JsonPropertyName("Win Rate %")]
    public string WinRate { get; set; } = null!;

    [JsonPropertyName("Matches")]
    public string Matches { get; set; } = null!;

    [JsonPropertyName("Average K/D Ratio")]
    public string AverageKDRatio { get; set; } = null!;

    [JsonPropertyName("Average Headshots %")]
    public string Headshots { get; set; } = null!;

    [JsonPropertyName("Average Kills")]
    public string AverageKills { get; set; } = null!;

    [JsonPropertyName("Average Deaths")]
    public string AverageDeaths { get; set; } = null!;

    [JsonPropertyName("Average Assists")]
    public string AverageAssists { get; set; } = null!;

    [JsonPropertyName("Average K/R Ratio")]
    public string AverageKRRatio { get; set; } = null!;

    [JsonPropertyName("Average Triple Kills")]
    public string AverageTripleKills { get; set; } = null!;

    [JsonPropertyName("Average Quadro Kills")]
    public string AverageQuadroKills { get; set; } = null!;

    [JsonPropertyName("Average Penta Kills")]
    public string AveragePentaKills { get; set; } = null!;
}
