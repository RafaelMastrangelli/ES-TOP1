using ESTop1.Domain;
using Xunit;

namespace ESTop1.Domain.Tests;

public class FiltrosTests
{
    [Fact]
    public void FiltroJogador_DeveTerValoresPadrao()
    {
        var filtro = new FiltroJogador();

        Assert.Equal(1, filtro.Page);
        Assert.Equal(20, filtro.PageSize);
        Assert.Equal("apelido_asc", filtro.Ordenar);
    }

    [Fact]
    public void FiltroTime_DeveTerValoresPadrao()
    {
        var filtro = new FiltroTime();

        Assert.Equal(1, filtro.Page);
        Assert.Equal(12, filtro.PageSize);
    }
}
