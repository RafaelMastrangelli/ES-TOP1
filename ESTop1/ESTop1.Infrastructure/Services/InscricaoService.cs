using ESTop1.Domain;
using ESTop1.Domain.DTOs;
using ESTop1.Domain.Interfaces;

namespace ESTop1.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de inscrições
/// </summary>
public class InscricaoService : IInscricaoService
{
    private readonly IJogadorRepository _jogadorRepository;

    public InscricaoService(IJogadorRepository jogadorRepository)
    {
        _jogadorRepository = jogadorRepository;
    }

    public async Task<InscricaoCriadaDto> CriarInscricaoAsync(CriarInscricaoCommand request, CancellationToken cancellationToken = default)
    {
        var jogadorId = Guid.NewGuid();
        var jogador = new Jogador
        {
            Id = jogadorId,
            Apelido = request.Apelido,
            Pais = request.Pais ?? "BR",
            Idade = request.Idade,
            FuncaoPrincipal = request.FuncaoPrincipal,
            Status = StatusJogador.Amador,
            Disponibilidade = Disponibilidade.Livre,
            ValorDeMercado = 0,
            Visivel = false
        };

        if (request.Rating.HasValue || request.KD.HasValue || request.PartidasJogadas.HasValue)
        {
            jogador.Estatisticas.Add(new Estatistica
            {
                Id = Guid.NewGuid(),
                JogadorId = jogadorId,
                Periodo = "Geral",
                Rating = request.Rating ?? 0,
                KD = request.KD ?? 0,
                PartidasJogadas = request.PartidasJogadas ?? 0
            });
        }

        await _jogadorRepository.CriarAsync(jogador, cancellationToken);

        return new InscricaoCriadaDto
        {
            InscricaoId = jogadorId,
            Message = "Inscrição criada com sucesso"
        };
    }

    public async Task<InscricaoMensagemDto> PagarInscricaoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var jogador = await _jogadorRepository.ObterPorIdAsync(id, cancellationToken);
        if (jogador is null)
        {
            throw new ArgumentException("Jogador não encontrado");
        }

        return new InscricaoMensagemDto { Message = "Pagamento processado com sucesso" };
    }

    public async Task<InscricaoMensagemDto> AprovarInscricaoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sucesso = await _jogadorRepository.AlterarVisibilidadeAsync(id, true, cancellationToken);
        if (!sucesso)
        {
            throw new ArgumentException("Jogador não encontrado");
        }

        return new InscricaoMensagemDto { Message = "Inscrição aprovada com sucesso" };
    }
}
