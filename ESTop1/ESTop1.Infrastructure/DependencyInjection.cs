using ESTop1.Infrastructure.Interfaces;
using ESTop1.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace ESTop1.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // Banco de dados
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(config.GetConnectionString("Padrao"), b => b.MigrationsAssembly("ESTop1.Infrastructure")));

        // FACEIT API
        services.AddHttpClient<IFaceitService, FaceitService>(client =>
        {
            client.BaseAddress = new Uri("https://open.faceit.com/data/v4/");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["Integracoes:FaceitToken"]}");
        });

        return services;
    }
}
