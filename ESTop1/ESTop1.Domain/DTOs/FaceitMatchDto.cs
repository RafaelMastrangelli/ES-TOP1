using System.Text.Json.Serialization;

namespace ESTop1.Domain.DTOs;

public class FaceitMatchDto
{
    [JsonPropertyName("match_id")]
    public string MatchId { get; set; } = null!;

    [JsonPropertyName("started_at")]
    public long StartedAt { get; set; }

    [JsonPropertyName("finished_at")]
    public long FinishedAt { get; set; }

    [JsonPropertyName("teams")]
    public FaceitTeamsDto? Teams { get; set; }

    [JsonPropertyName("winner")]
    public string Winner { get; set; } = null!;

    [JsonPropertyName("results")]
    public FaceitResultsDto? Results { get; set; }
}

public class FaceitTeamsDto
{
    [JsonPropertyName("faction1")]
    public FaceitTeamDto? Faction1 { get; set; }

    [JsonPropertyName("faction2")]
    public FaceitTeamDto? Faction2 { get; set; }
}

public class FaceitTeamDto
{
    [JsonPropertyName("team_id")]
    public string TeamId { get; set; } = null!;

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = null!;

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = null!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;

    [JsonPropertyName("players")]
    public List<FaceitPlayerMatchDto> Players { get; set; } = new();
}

public class FaceitPlayerMatchDto
{
    [JsonPropertyName("player_id")]
    public string PlayerId { get; set; } = null!;

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = null!;

    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = null!;

    [JsonPropertyName("game_player_id")]
    public string GamePlayerId { get; set; } = null!;

    [JsonPropertyName("game_player_name")]
    public string GamePlayerName { get; set; } = null!;

    [JsonPropertyName("faceit_url")]
    public string FaceitUrl { get; set; } = null!;
}

public class FaceitResultsDto
{
    [JsonPropertyName("score")]
    public FaceitScoreDto? Score { get; set; }
}

public class FaceitScoreDto
{
    [JsonPropertyName("faction1")]
    public int Faction1 { get; set; }

    [JsonPropertyName("faction2")]
    public int Faction2 { get; set; }
}
