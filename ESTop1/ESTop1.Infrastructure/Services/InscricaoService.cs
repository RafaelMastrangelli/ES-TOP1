using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de inscrições
/// </summary>
public class InscricaoService : IInscricaoService
{
    private readonly IJogadorRepository _jogadorRepository;
    private readonly ITimeRepository _timeRepository;
    private readonly AppDbContext _context;

    public InscricaoService(IJogadorRepository jogadorRepository, ITimeRepository timeRepository, AppDbContext context)
    {
        _jogadorRepository = jogadorRepository;
        _timeRepository = timeRepository;
        _context = context;
    }

    public async Task<object> CriarInscricaoAsync(object requestObj, CancellationToken cancellationToken = default)
    {
        var request = (dynamic)requestObj;
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

        // Adicionar estatística geral se fornecida
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

        return new { inscricaoId = jogadorId, message = "Inscrição criada com sucesso" };
    }

    public async Task<object> PagarInscricaoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var jogador = await _jogadorRepository.ObterPorIdAsync(id, cancellationToken);
        if (jogador == null)
        {
            throw new ArgumentException("Jogador não encontrado");
        }

        // Mock: apenas retorna sucesso
        return new { message = "Pagamento processado com sucesso" };
    }

    public async Task<object> AprovarInscricaoAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sucesso = await _jogadorRepository.AlterarVisibilidadeAsync(id, true, cancellationToken);
        
        if (!sucesso)
        {
            throw new ArgumentException("Jogador não encontrado");
        }

        return new { message = "Inscrição aprovada com sucesso" };
    }
}
