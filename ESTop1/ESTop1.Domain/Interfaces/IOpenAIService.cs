namespace ESTop1.Domain.Interfaces;

public interface IOpenAIService
{
    Task<(int StatusCode, object Payload)> BuscarJogadoresAsync(string consulta, CancellationToken cancellationToken = default);
    Task<(int StatusCode, object Payload)> SugerirFiltrosAsync(string descricao, CancellationToken cancellationToken = default);
}
