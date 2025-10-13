using ESTop1.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Numerics;

namespace ESTop1.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Jogador> Jogadores => Set<Jogador>();
    public DbSet<Estatistica> Estatisticas => Set<Estatistica>();
    public DbSet<Time> Times => Set<Time>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Jogador>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Apelido);
            e.Property(x => x.ValorDeMercado).HasPrecision(14, 2);
            e.HasOne(x => x.TimeAtual).WithMany(t => t.Jogadores).HasForeignKey(x => x.TimeAtualId);
        });

        b.Entity<Estatistica>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.JogadorId, x.Periodo });
        });

        b.Entity<Time>(e => e.HasKey(x => x.Id));
    }

    // Dados iniciais (seed)
    public static void PopularBanco(AppDbContext db)
    {
        if (db.Jogadores.Any()) return;

        // Criar time
        var time = new Time { Id = Guid.NewGuid(), Nome = "Bauru Stars", Pais = "BR" };
        db.Times.Add(time);

        // Criar jogador em time
        var jogador1 = new Jogador
        {
            Id = Guid.NewGuid(),
            Apelido = "rafaa",
            Pais = "BR",
            Idade = 24,
            FuncaoPrincipal = Funcao.Entry,
            Status = StatusJogador.Profissional,
            Disponibilidade = Disponibilidade.EmTime,
            TimeAtualId = time.Id,
            Visivel = true
        };

        var estatistica1 = new Estatistica
        {
            Id = Guid.NewGuid(),
            JogadorId = jogador1.Id,
            Periodo = "Geral",
            Rating = 1.12m,
            KD = 1.08m,
            PartidasJogadas = 250
        };

        jogador1.ValorDeMercado = CalcularValorMercado(estatistica1.Rating, estatistica1.KD, estatistica1.PartidasJogadas);

        // Criar jogador livre
        var jogador2 = new Jogador
        {
            Id = Guid.NewGuid(),
            Apelido = "coldzera",
            Pais = "BR",
            Idade = 28,
            FuncaoPrincipal = Funcao.Awp,
            Status = StatusJogador.Profissional,
            Disponibilidade = Disponibilidade.Livre,
            Visivel = true
        };

        var estatistica2 = new Estatistica
        {
            Id = Guid.NewGuid(),
            JogadorId = jogador2.Id,
            Periodo = "Geral",
            Rating = 1.25m,
            KD = 1.15m,
            PartidasJogadas = 400
        };

        jogador2.ValorDeMercado = CalcularValorMercado(estatistica2.Rating, estatistica2.KD, estatistica2.PartidasJogadas);

        db.Jogadores.AddRange(jogador1, jogador2);
        db.Estatisticas.AddRange(estatistica1, estatistica2);
        db.SaveChanges();
    }

    /// <summary>
    /// Calcula valor de mercado baseado nas estatísticas
    /// Fórmula: 50_000 * (rating^1.5) * (1 + kd/10) * (1 + partidas/500)
    /// Clamp entre 10_000 e 5_000_000
    /// </summary>
    public static decimal CalcularValorMercado(decimal rating, decimal kd, int partidas)
    {
        var valor = 50_000m * (decimal)Math.Pow((double)rating, 1.5) * (1 + kd / 10) * (1 + partidas / 500m);
        return Math.Max(10_000m, Math.Min(5_000_000m, valor));
    }
}
