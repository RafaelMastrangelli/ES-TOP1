using ESTop1.Domain;
using ESTop1.Domain.DTOs;
using ESTop1.Domain.Interfaces;

namespace ESTop1.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de jogadores
/// </summary>
public class JogadorService : IJogadorService
{
    private readonly IJogadorRepository _jogadorRepository;

    public JogadorService(IJogadorRepository jogadorRepository)
    {
        _jogadorRepository = jogadorRepository;
    }

    public async Task<JogadoresPaginadosDto> ListarJogadoresAsync(FiltroJogador filtro, CancellationToken cancellationToken = default)
    {
        var jogadores = await _jogadorRepository.ListarAsync(filtro, cancellationToken);
        var total = await _jogadorRepository.ContarAsync(filtro, cancellationToken);

        return new JogadoresPaginadosDto
        {
            Total = total,
            Items = jogadores.Select(MapearListagem).ToList()
        };
    }

    public async Task<JogadorDetalheDto?> ObterJogadorPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var jogador = await _jogadorRepository.ObterPorIdAsync(id, cancellationToken);
        return jogador is null ? null : MapearDetalhe(jogador);
    }

    public async Task<JogadorDetalheDto?> ObterJogadorPorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        var jogador = await _jogadorRepository.ObterPorIdAsync(usuarioId, cancellationToken);
        return jogador is null ? null : MapearDetalhe(jogador, incluirDetalhesTime: true);
    }

    public async Task<JogadorResumoDto> CriarJogadorAsync(CriarJogadorCommand request, CancellationToken cancellationToken = default)
    {
        var jogador = new Jogador
        {
            Id = Guid.NewGuid(),
            Apelido = request.Apelido,
            Pais = request.Pais ?? "BR",
            Idade = request.Idade,
            FuncaoPrincipal = request.FuncaoPrincipal,
            Status = request.Status,
            Disponibilidade = request.Disponibilidade,
            TimeAtualId = request.TimeAtualId,
            ValorDeMercado = request.ValorDeMercado,
            Visivel = true
        };

        var jogadorCriado = await _jogadorRepository.CriarAsync(jogador, cancellationToken);
        return MapearResumo(jogadorCriado);
    }

    public async Task<JogadorResumoDto> CriarJogadorParaUsuarioAsync(Guid usuarioId, string nome, CancellationToken cancellationToken = default)
    {
        var jogador = new Jogador
        {
            Id = usuarioId,
            Apelido = nome,
            Pais = "BR",
            Idade = 18,
            FuncaoPrincipal = Funcao.Entry,
            Status = StatusJogador.Amador,
            Disponibilidade = Disponibilidade.Livre,
            ValorDeMercado = 10000,
            Visivel = true
        };

        var jogadorCriado = await _jogadorRepository.CriarAsync(jogador, cancellationToken);
        return MapearResumo(jogadorCriado);
    }

    public async Task<JogadorDetalheDto?> AtualizarJogadorAsync(Guid usuarioId, AtualizarJogadorCommand request, CancellationToken cancellationToken = default)
    {
        var jogador = await _jogadorRepository.ObterRastreadoPorIdAsync(usuarioId, cancellationToken);
        if (jogador is null) return null;

        if (request.Apelido is not null) jogador.Apelido = request.Apelido;
        if (request.Pais is not null) jogador.Pais = request.Pais;
        if (request.Idade.HasValue) jogador.Idade = request.Idade.Value;
        if (request.FuncaoPrincipal.HasValue) jogador.FuncaoPrincipal = request.FuncaoPrincipal.Value;
        if (request.Status.HasValue) jogador.Status = request.Status.Value;
        if (request.Disponibilidade.HasValue) jogador.Disponibilidade = request.Disponibilidade.Value;
        if (request.ValorDeMercado.HasValue) jogador.ValorDeMercado = request.ValorDeMercado.Value;
        if (request.FotoUrl is not null) jogador.FotoUrl = request.FotoUrl;

        var jogadorAtualizado = await _jogadorRepository.AtualizarAsync(jogador, cancellationToken);
        return MapearDetalhe(jogadorAtualizado, incluirDetalhesTime: true);
    }

    public Task<bool> AlterarVisibilidadeJogadorAsync(Guid id, bool visivel, CancellationToken cancellationToken = default)
    {
        return _jogadorRepository.AlterarVisibilidadeAsync(id, visivel, cancellationToken);
    }

    public async Task<AtualizarFotosResultDto> AtualizarFotosJogadoresAsync(CancellationToken cancellationToken = default)
    {
        var total = await _jogadorRepository.AtualizarFotosAusentesAsync(
            apelido => $"https://via.placeholder.com/300x300/1a1a1a/ffffff?text={Uri.EscapeDataString(apelido)}",
            cancellationToken);

        return new AtualizarFotosResultDto
        {
            Message = $"Fotos atualizadas para {total} jogadores",
            TotalAtualizado = total
        };
    }

    private static JogadorListagemDto MapearListagem(Jogador jogador)
    {
        var ratingGeral = jogador.Estatisticas
            .FirstOrDefault(e => e.Periodo == "Geral")?.Rating ?? 0;

        return new JogadorListagemDto
        {
            Id = jogador.Id,
            Apelido = jogador.Apelido,
            Pais = jogador.Pais,
            Idade = jogador.Idade,
            Time = jogador.TimeAtual?.Nome,
            FuncaoPrincipal = jogador.FuncaoPrincipal,
            Status = jogador.Status,
            Disponibilidade = jogador.Disponibilidade,
            ValorDeMercado = jogador.ValorDeMercado,
            FotoUrl = jogador.FotoUrl,
            RatingGeral = ratingGeral
        };
    }

    private static JogadorResumoDto MapearResumo(Jogador jogador)
    {
        return new JogadorResumoDto
        {
            Id = jogador.Id,
            Apelido = jogador.Apelido,
            Pais = jogador.Pais,
            Idade = jogador.Idade,
            FuncaoPrincipal = jogador.FuncaoPrincipal,
            Status = jogador.Status,
            Disponibilidade = jogador.Disponibilidade,
            ValorDeMercado = jogador.ValorDeMercado,
            Visivel = jogador.Visivel,
            TimeAtual = jogador.TimeAtual is null ? null : MapearTime(jogador.TimeAtual)
        };
    }

    private static JogadorDetalheDto MapearDetalhe(Jogador jogador, bool incluirDetalhesTime = false)
    {
        return new JogadorDetalheDto
        {
            Id = jogador.Id,
            Apelido = jogador.Apelido,
            Pais = jogador.Pais,
            Idade = jogador.Idade,
            FuncaoPrincipal = jogador.FuncaoPrincipal,
            Status = jogador.Status,
            Disponibilidade = jogador.Disponibilidade,
            ValorDeMercado = jogador.ValorDeMercado,
            FotoUrl = jogador.FotoUrl,
            Visivel = jogador.Visivel,
            TimeAtual = jogador.TimeAtual is null
                ? null
                : MapearTime(jogador.TimeAtual, incluirDetalhesTime),
            Estatisticas = jogador.Estatisticas
                .Select(e => new EstatisticaDto
                {
                    Id = e.Id,
                    Periodo = e.Periodo,
                    Rating = e.Rating,
                    KD = e.KD,
                    PartidasJogadas = e.PartidasJogadas
                })
                .ToList()
        };
    }

    private static TimeResumoDto MapearTime(Time time, bool incluirDetalhes = false)
    {
        return new TimeResumoDto
        {
            Id = time.Id,
            Nome = time.Nome,
            Pais = time.Pais,
            Tier = incluirDetalhes ? time.Tier : null,
            Contratando = incluirDetalhes ? time.Contratando : null
        };
    }
}
