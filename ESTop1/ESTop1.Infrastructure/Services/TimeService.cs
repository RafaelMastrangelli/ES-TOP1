using ESTop1.Domain;
using ESTop1.Domain.DTOs;
using ESTop1.Domain.Interfaces;

namespace ESTop1.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de times
/// </summary>
public class TimeService : ITimeService
{
    private readonly ITimeRepository _timeRepository;
    private readonly IUsuarioRepository _usuarioRepository;

    public TimeService(ITimeRepository timeRepository, IUsuarioRepository usuarioRepository)
    {
        _timeRepository = timeRepository;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<TimesPaginadosDto> ListarTimesAsync(FiltroTime filtros, CancellationToken cancellationToken = default)
    {
        var times = await _timeRepository.ListarAsync(filtros, cancellationToken);
        var total = await _timeRepository.ContarAsync(filtros, cancellationToken);

        return new TimesPaginadosDto
        {
            Total = total,
            Page = filtros.Page,
            PageSize = filtros.PageSize,
            Items = times.Select(MapearListagem).ToList()
        };
    }

    public async Task<TimeDetalheDto?> ObterTimePorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var time = await _timeRepository.ObterPorIdAsync(id, cancellationToken);
        return time is null ? null : MapearDetalhe(time);
    }

    public async Task<TimeDetalheDto?> ObterTimePorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario?.TimeId is null) return null;

        var time = await _timeRepository.ObterPorIdAsync(usuario.TimeId.Value, cancellationToken);
        return time is null ? null : MapearDetalhe(time);
    }

    public async Task<TimeDetalheDto> CriarTimeAsync(Guid usuarioId, CriarTimeCommand request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId)
            ?? throw new InvalidOperationException("Usuário não encontrado");

        if (usuario.TimeId is not null)
            throw new InvalidOperationException("Usuário já possui um time cadastrado");

        var time = new Time
        {
            Id = Guid.NewGuid(),
            Nome = request.Nome,
            Pais = request.Pais ?? "BR"
        };

        var timeCriado = await _timeRepository.CriarAsync(time, cancellationToken);

        usuario.TimeId = timeCriado.Id;
        await _usuarioRepository.AtualizarAsync(usuario);

        return MapearDetalhe(timeCriado);
    }

    public async Task<TimeDetalheDto?> AtualizarTimeAsync(Guid usuarioId, AtualizarTimeCommand request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioId);
        if (usuario?.TimeId is null) return null;

        var time = await _timeRepository.ObterRastreadoPorIdAsync(usuario.TimeId.Value, cancellationToken);
        if (time is null) return null;

        if (request.Nome is not null) time.Nome = request.Nome;
        if (request.Pais is not null) time.Pais = request.Pais;
        if (request.Tier.HasValue) time.Tier = request.Tier;
        if (request.Contratando.HasValue) time.Contratando = request.Contratando;
        if (request.LogoUrl is not null) time.LogoUrl = request.LogoUrl;

        var timeAtualizado = await _timeRepository.AtualizarAsync(time, cancellationToken);
        return MapearDetalhe(timeAtualizado);
    }

    private static TimeListagemDto MapearListagem(Time time)
    {
        return new TimeListagemDto
        {
            Id = time.Id,
            Nome = time.Nome,
            Pais = time.Pais,
            Tier = time.Tier,
            Contratando = time.Contratando,
            QuantidadeJogadores = time.Jogadores.Count
        };
    }

    private static TimeDetalheDto MapearDetalhe(Time time)
    {
        return new TimeDetalheDto
        {
            Id = time.Id,
            Nome = time.Nome,
            Pais = time.Pais,
            Tier = time.Tier,
            Contratando = time.Contratando,
            LogoUrl = time.LogoUrl,
            Jogadores = time.Jogadores.Select(j => new JogadorNoTimeDto
            {
                Id = j.Id,
                Apelido = j.Apelido,
                Pais = j.Pais,
                Idade = j.Idade,
                FuncaoPrincipal = j.FuncaoPrincipal,
                Status = j.Status,
                Disponibilidade = j.Disponibilidade,
                ValorDeMercado = j.ValorDeMercado,
                FotoUrl = j.FotoUrl
            }).ToList()
        };
    }
}
