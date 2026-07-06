using ESTop1.Domain.DTOs;

namespace ESTop1.Domain.Interfaces;

/// <summary>
/// Interface para serviço de inscrições
/// </summary>
public interface IInscricaoService
{
    Task<InscricaoCriadaDto> CriarInscricaoAsync(CriarInscricaoCommand request, CancellationToken cancellationToken = default);
    Task<InscricaoMensagemDto> PagarInscricaoAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InscricaoMensagemDto> AprovarInscricaoAsync(Guid id, CancellationToken cancellationToken = default);
}
