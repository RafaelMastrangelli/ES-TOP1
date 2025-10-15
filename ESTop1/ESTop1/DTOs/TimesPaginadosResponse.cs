using ESTop1.Domain;

namespace ESTop1.Api.DTOs;

public class TimesPaginadosResponse
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<Time> Items { get; set; } = new();
}
