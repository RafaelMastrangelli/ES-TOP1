namespace ESTop1.Api.DTOs;

public class FiltroTime
{
    public string? Nome { get; set; }
    public int? Tier { get; set; }
    public bool? Contratando { get; set; }
    public string? Ordenar { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
