using ESTop1.Domain.DTOs;

namespace ESTop1.Infrastructure.Interfaces;

public interface IFaceitService
{
    Task<FaceitPlayerDto?> ObterJogadorPorNicknameAsync(string nickname);
    Task<FaceitStatsDto?> ObterEstatisticasDoJogadorAsync(string playerId);
    Task<IEnumerable<FaceitMatchDto>> ObterUltimasPartidasAsync(string playerId, int limite = 5);
}
