using ESTop1.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Azure.AI.OpenAI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Infrastructure (banco de dados + FACEIT API)
builder.Services.AddInfrastructure(builder.Configuration);

// OpenAI Configuration
var openAIApiKey = builder.Configuration["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (!string.IsNullOrEmpty(openAIApiKey))
{
    builder.Services.AddSingleton(new OpenAIClient(openAIApiKey));
}

var app = builder.Build();

// Aplicar migra��es e popular banco
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
            // Verificar se o banco existe e criar se necessário
            try
            {
                db.Database.EnsureCreated();
                AppDbContext.PopularBanco(db);
                
                // Atualizar fotos dos jogadores existentes
                var jogadoresSemFoto = db.Jogadores.Where(j => string.IsNullOrEmpty(j.FotoUrl)).ToList();
                foreach (var jogador in jogadoresSemFoto)
                {
                    jogador.FotoUrl = $"https://via.placeholder.com/300x300/1a1a1a/ffffff?text={Uri.EscapeDataString(jogador.Apelido)}";
                }
                if (jogadoresSemFoto.Any())
                {
                    db.SaveChanges();
                    Console.WriteLine($"Fotos atualizadas para {jogadoresSemFoto.Count} jogadores");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inicializar banco: {ex.Message}");
            }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS
app.UseCors("AllowFrontend");

app.MapControllers();
app.Run();
