using ESTop1.Infrastructure;
using ESTop1.Api.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = false;
    });
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:5173", "http://localhost:3000", "http://localhost:8080" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Adicionar suporte a credenciais
    });
});

// Infrastructure (banco de dados + FACEIT API)
builder.Services.AddInfrastructure(builder.Configuration);

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    if (builder.Environment.IsDevelopment())
    {
        jwtKey = "ESTop1_Dev_Key_Only_For_Local_Development";
    }
    else
    {
        throw new InvalidOperationException(
            "Jwt:Key deve ser configurada em produção (variável de ambiente ou appsettings).");
    }
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ESTop1";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ESTop1Client";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Aplicar migra��es e popular banco
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        await db.Database.MigrateAsync();
        AppDbContext.PopularBanco(db, app.Environment.IsDevelopment());

        var jogadoresSemFoto = db.Jogadores
            .Where(j => string.IsNullOrEmpty(j.FotoUrl) || j.FotoUrl.Contains("placeholder"))
            .ToList();

        foreach (var jogador in jogadoresSemFoto)
        {
            jogador.FotoUrl = "/player-default.jpg";
        }

        if (jogadoresSemFoto.Count > 0)
        {
            await db.SaveChangesAsync();
            logger.LogInformation("Fotos atualizadas para {Count} jogadores", jogadoresSemFoto.Count);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao inicializar banco de dados");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.UseExceptionHandler();
app.UseMiddleware<SecurityHeadersMiddleware>();

// CORS
app.UseCors("AllowFrontend");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Middleware de assinatura
app.UseMiddleware<AssinaturaMiddleware>();

app.MapControllers();
app.Run();

public partial class Program { }
