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
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Assinatura> Assinaturas => Set<Assinatura>();
    public DbSet<Plano> Planos => Set<Plano>();

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

        b.Entity<Usuario>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(255);
            e.Property(x => x.Nome).HasMaxLength(100);
            e.HasOne(x => x.Time).WithMany().HasForeignKey(x => x.TimeId);
        });

        b.Entity<Assinatura>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UsuarioId, x.Status });
            e.Property(x => x.ValorMensal).HasPrecision(10, 2);
            e.HasOne(x => x.Usuario).WithMany(u => u.Assinaturas).HasForeignKey(x => x.UsuarioId);
        });

        b.Entity<Plano>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Tipo).IsUnique();
            e.Property(x => x.ValorMensal).HasPrecision(10, 2);
        });
    }

    // Dados iniciais (seed)
    public static void PopularBanco(AppDbContext db)
    {
        if (db.Jogadores.Any()) return;

        // Criar planos de assinatura
        var planos = new[]
        {
            new Plano
            {
                Id = Guid.NewGuid(),
                Tipo = PlanoAssinatura.Gratuito,
                Nome = "Gratuito",
                Descricao = "Perfeito para começar e testar a plataforma",
                ValorMensal = 0,
                LimiteJogadores = 5,
                AcessoEstatisticas = true,
                AcessoBuscaIA = false,
                AcessoAPI = false,
                SuportePrioritario = false
            },
            new Plano
            {
                Id = Guid.NewGuid(),
                Tipo = PlanoAssinatura.Mensal,
                Nome = "Mensal",
                Descricao = "Ideal para uso regular e descoberta de talentos",
                ValorMensal = 100,
                LimiteJogadores = 50,
                AcessoEstatisticas = true,
                AcessoBuscaIA = true,
                AcessoAPI = false,
                SuportePrioritario = false
            },
            new Plano
            {
                Id = Guid.NewGuid(),
                Tipo = PlanoAssinatura.Trimestral,
                Nome = "Trimestral",
                Descricao = "Melhor custo-benefício para times profissionais",
                ValorMensal = 230,
                LimiteJogadores = -1, // Ilimitado
                AcessoEstatisticas = true,
                AcessoBuscaIA = true,
                AcessoAPI = true,
                SuportePrioritario = true
            },
            new Plano
            {
                Id = Guid.NewGuid(),
                Tipo = PlanoAssinatura.Enterprise,
                Nome = "Enterprise",
                Descricao = "Para organizações grandes",
                ValorMensal = 499.90m,
                LimiteJogadores = -1, // Ilimitado
                AcessoEstatisticas = true,
                AcessoBuscaIA = true,
                AcessoAPI = true,
                SuportePrioritario = true
            }
        };

        db.Planos.AddRange(planos);

        // Criar usuário admin
        var admin = new Usuario
        {
            Id = Guid.NewGuid(),
            Nome = "Administrador",
            Email = "admin@estop1.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Tipo = TipoUsuario.Admin,
            Ativo = true
        };

        db.Usuarios.Add(admin);

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
            Disponibilidade = Disponibilidade.Contratado,
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
