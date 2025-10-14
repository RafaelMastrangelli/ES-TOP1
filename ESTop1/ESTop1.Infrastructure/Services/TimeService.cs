using ESTop1.Domain;
using ESTop1.Domain.Interfaces;

namespace ESTop1.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de times
/// </summary>
public class TimeService : ITimeService
{
    private readonly ITimeRepository _timeRepository;

    public TimeService(ITimeRepository timeRepository)
    {
        _timeRepository = timeRepository;
    }

    public async Task<IEnumerable<object>> ListarTimesAsync(CancellationToken cancellationToken = default)
    {
        var times = await _timeRepository.ListarAsync(cancellationToken);

        return times.Select(t => new
        {
            t.Id,
            t.Nome,
            t.Pais,
            QuantidadeJogadores = t.Jogadores.Count
        });
    }

    public async Task<object?> ObterTimePorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var time = await _timeRepository.ObterPorIdAsync(id, cancellationToken);
        
        if (time == null) return null;

        return time;
    }

    public async Task<object> CriarTimeAsync(object requestObj, CancellationToken cancellationToken = default)
    {
        var request = (dynamic)requestObj;
        var time = new Time
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Pais = request.Pais ?? "BR"
        };

        var timeCriado = await _timeRepository.CriarAsync(time, cancellationToken);
        return timeCriado;
    }
}
