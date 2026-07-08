using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> ObterAtivoPorHashAsync(string tokenHash)
    {
        return await _context.RefreshTokens
            .Include(r => r.Usuario)
            .FirstOrDefaultAsync(r => r.TokenHash == tokenHash && r.RevokedAt == null);
    }

    public async Task<RefreshToken> CriarAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task AtualizarAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevogarTodosDoUsuarioAsync(Guid usuarioId)
    {
        var tokens = await _context.RefreshTokens
            .Where(r => r.UsuarioId == usuarioId && r.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        if (tokens.Count > 0)
        {
            await _context.SaveChangesAsync();
        }
    }
}
