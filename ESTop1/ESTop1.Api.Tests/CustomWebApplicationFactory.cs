using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ESTop1.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath = Path.Combine(
        Path.GetTempPath(),
        $"estop1-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Padrao"] = $"Data Source={_databasePath}",
                ["ASPNETCORE_ENVIRONMENT"] = "Development",
                ["Jwt:Key"] = "ESTop1_Dev_Key_Only_For_Local_Development",
                ["Jwt:Issuer"] = "ESTop1",
                ["Jwt:Audience"] = "ESTop1Client",
                ["MercadoPago:AccessToken"] = "",
                ["Integracoes:FaceitToken"] = "",
                ["LLM:ApiKey"] = ""
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IHostLifetime, NoOpHostLifetime>();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing || !File.Exists(_databasePath))
        {
            return;
        }

        try
        {
            File.Delete(_databasePath);
        }
        catch (IOException)
        {
            // SQLite pode manter lock breve após dispose do host.
        }
    }

    private sealed class NoOpHostLifetime : IHostLifetime
    {
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task WaitForStartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
