using ESTop1.Domain.DTOs;
using ESTop1.Infrastructure.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ESTop1.Infrastructure.Services;

public class FaceitService : IFaceitService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<FaceitService> _logger;

    public FaceitService(HttpClient httpClient, ILogger<FaceitService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<FaceitPlayerDto?> ObterJogadorPorNicknameAsync(string nickname)
    {
        try
        {
            _logger.LogInformation("Buscando jogador FACEIT por nickname: {Nickname}", nickname);
            
            var response = await _httpClient.GetAsync($"players?nickname={Uri.EscapeDataString(nickname)}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao buscar jogador {Nickname}. Status: {StatusCode}", nickname, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var player = JsonSerializer.Deserialize<FaceitPlayerDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Jogador {Nickname} encontrado com sucesso", nickname);
            return player;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar jogador {Nickname}", nickname);
            return null;
        }
    }

    public async Task<FaceitStatsDto?> ObterEstatisticasDoJogadorAsync(string playerId)
    {
        try
        {
            _logger.LogInformation("Buscando estatísticas do jogador FACEIT: {PlayerId}", playerId);
            
            var response = await _httpClient.GetAsync($"players/{playerId}/stats/cs2");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao buscar estatísticas do jogador {PlayerId}. Status: {StatusCode}", playerId, response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var stats = JsonSerializer.Deserialize<FaceitStatsDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogInformation("Estatísticas do jogador {PlayerId} obtidas com sucesso", playerId);
            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estatísticas do jogador {PlayerId}", playerId);
            return null;
        }
    }

    public async Task<IEnumerable<FaceitMatchDto>> ObterUltimasPartidasAsync(string playerId, int limite = 5)
    {
        try
        {
            _logger.LogInformation("Buscando últimas {Limite} partidas do jogador FACEIT: {PlayerId}", limite, playerId);
            
            var response = await _httpClient.GetAsync($"players/{playerId}/history?game=cs2&limit={limite}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao buscar partidas do jogador {PlayerId}. Status: {StatusCode}", playerId, response.StatusCode);
                return Enumerable.Empty<FaceitMatchDto>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var historyResponse = JsonSerializer.Deserialize<FaceitHistoryResponseDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var matches = historyResponse?.Items ?? Enumerable.Empty<FaceitMatchDto>();
            
            _logger.LogInformation("Encontradas {Count} partidas para o jogador {PlayerId}", matches.Count(), playerId);
            return matches;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar partidas do jogador {PlayerId}", playerId);
            return Enumerable.Empty<FaceitMatchDto>();
        }
    }
}

public class FaceitHistoryResponseDto
{
    public List<FaceitMatchDto> Items { get; set; } = new();
}
