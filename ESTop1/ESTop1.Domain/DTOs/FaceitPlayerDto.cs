using System.Text.Json.Serialization;

namespace ESTop1.Domain.DTOs;

public class FaceitPlayerDto
{
    [JsonPropertyName("player_id")]
    public string PlayerId { get; set; } = null!;

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = null!;

    [JsonPropertyName("country")]
    public string Country { get; set; } = null!;

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = null!;

    [JsonPropertyName("games")]
    public FaceitGamesDto? Games { get; set; }
}

public class FaceitGamesDto
{
    [JsonPropertyName("cs2")]
    public FaceitCs2Dto? Cs2 { get; set; }
}

public class FaceitCs2Dto
{
    [JsonPropertyName("skill_level")]
    public int SkillLevel { get; set; }

    [JsonPropertyName("faceit_elo")]
    public int FaceitElo { get; set; }
}
