using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure.Interfaces;
using ESTop1.Infrastructure.Repositories;
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

        // Repositórios
        services.AddScoped<IJogadorRepository, JogadorRepository>();
        services.AddScoped<ITimeRepository, TimeRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // Serviços
        services.AddScoped<IJogadorService, JogadorService>();
        services.AddScoped<ITimeService, TimeService>();
        services.AddScoped<IInscricaoService, InscricaoService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAssinaturaService, AssinaturaService>();

        // FACEIT API
        services.AddHttpClient<IFaceitService, FaceitService>(client =>
        {
            client.BaseAddress = new Uri("https://open.faceit.com/data/v4/");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["Integracoes:FaceitToken"]}");
        });

        return services;
    }
}
