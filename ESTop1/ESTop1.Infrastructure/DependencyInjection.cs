using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure.Repositories;
using ESTop1.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ESTop1.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(config.GetConnectionString("Padrao"), b => b.MigrationsAssembly("ESTop1.Infrastructure")));

        services.AddScoped<IJogadorRepository, JogadorRepository>();
        services.AddScoped<ITimeRepository, TimeRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IAssinaturaRepository, AssinaturaRepository>();
        services.AddScoped<IPlanoRepository, PlanoRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPagamentoRepository, PagamentoRepository>();

        services.AddScoped<IJogadorService, JogadorService>();
        services.AddScoped<ITimeService, TimeService>();
        services.AddScoped<IInscricaoService, InscricaoService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAssinaturaService, AssinaturaService>();
        services.AddScoped<IPagamentoService, PagamentoService>();
        services.AddScoped<IOpenAIService, OpenAIService>();

        services.AddHttpClient("MercadoPago", client =>
        {
            client.BaseAddress = new Uri("https://api.mercadopago.com/");
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        var provider = config["LLM:Provider"] ?? "OpenAI";
        var llmBaseUrl = provider.Equals("Groq", StringComparison.OrdinalIgnoreCase)
            ? "https://api.groq.com/openai/v1"
            : config["LLM:BaseUrl"] ?? "https://api.openai.com/v1";

        services.AddHttpClient<ILLMChatService, OpenAiCompatibleChatService>(client =>
        {
            client.BaseAddress = new Uri(llmBaseUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromSeconds(60);
        });

        services.AddHttpClient<IFaceitService, FaceitService>(client =>
        {
            client.BaseAddress = new Uri("https://open.faceit.com/data/v4/");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {config["Integracoes:FaceitToken"]}");
        });

        return services;
    }
}
