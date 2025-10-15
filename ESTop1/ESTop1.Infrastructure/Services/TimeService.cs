using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de times
/// </summary>
public class TimeService : ITimeService
{
    private readonly ITimeRepository _timeRepository;
    private readonly AppDbContext _context;

    public TimeService(ITimeRepository timeRepository, AppDbContext context)
    {
        _timeRepository = timeRepository;
        _context = context;
    }

    public async Task<object> ListarTimesAsync(object filtrosObj, CancellationToken cancellationToken = default)
    {
        var times = await _timeRepository.ListarComFiltrosAsync(filtrosObj, cancellationToken);
        
        // Converter para lista para trabalhar sem problemas de dynamic
        var timesList = times.ToList();
        
        // Aplicar filtros manualmente
        var timesFiltrados = timesList.AsEnumerable();
        
        // Filtro por nome
        if (filtrosObj.GetType().GetProperty("Nome")?.GetValue(filtrosObj) is string nome && !string.IsNullOrEmpty(nome))
        {
            timesFiltrados = timesFiltrados.Where(t => t.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase));
        }
        
        // Filtro por tier
        if (filtrosObj.GetType().GetProperty("Tier")?.GetValue(filtrosObj) is int tier)
        {
            timesFiltrados = timesFiltrados.Where(t => t.Tier == tier);
        }
        
        // Filtro por contratando
        if (filtrosObj.GetType().GetProperty("Contratando")?.GetValue(filtrosObj) is bool contratando)
        {
            timesFiltrados = timesFiltrados.Where(t => t.Contratando == contratando);
        }
        
        // Aplicar ordenação
        var ordenar = filtrosObj.GetType().GetProperty("Ordenar")?.GetValue(filtrosObj) as string;
        timesFiltrados = ordenar switch
        {
            "nome_asc" => timesFiltrados.OrderBy(t => t.Nome),
            "tier_asc" => timesFiltrados.OrderBy(t => t.Tier ?? int.MaxValue),
            "tier_desc" => timesFiltrados.OrderByDescending(t => t.Tier ?? 0),
            _ => timesFiltrados.OrderBy(t => t.Nome)
        };
        
        // Aplicar paginação
        var total = timesFiltrados.Count();
        var page = (int)(filtrosObj.GetType().GetProperty("Page")?.GetValue(filtrosObj) ?? 1);
        var pageSize = (int)(filtrosObj.GetType().GetProperty("PageSize")?.GetValue(filtrosObj) ?? 12);
        var skip = (page - 1) * pageSize;
        
        var items = timesFiltrados.Skip(skip).Take(pageSize).Select(t => new
        {
            t.Id,
            t.Nome,
            t.Pais,
            t.Tier,
            t.Contratando,
            QuantidadeJogadores = t.Jogadores.Count
        }).ToList();
        
        return new
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public async Task<object?> ObterTimePorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var time = await _timeRepository.ObterPorIdAsync(id, cancellationToken);
        
        if (time == null) return null;

        return time;
    }

    public async Task<object?> ObterTimePorUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        // Para organizações, vamos buscar o time associado ao usuário através do TimeId
        var usuario = await _context.Usuarios
            .Include(u => u.Time)
            .FirstOrDefaultAsync(u => u.Id == usuarioId, cancellationToken);
        
        if (usuario?.Time == null) return null;
        
        var time = await _context.Times
            .Include(t => t.Jogadores)
            .FirstOrDefaultAsync(t => t.Id == usuario.TimeId, cancellationToken);
        
        if (time == null) return null;

        return new
        {
            time.Id,
            time.Nome,
            time.Pais,
            time.Tier,
            time.Contratando,
            Jogadores = time.Jogadores.Select(j => new
            {
                j.Id,
                j.Apelido,
                j.Pais,
                j.Idade,
                j.FuncaoPrincipal,
                j.Status,
                j.Disponibilidade,
                j.ValorDeMercado,
                j.FotoUrl
            }).ToList()
        };
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
