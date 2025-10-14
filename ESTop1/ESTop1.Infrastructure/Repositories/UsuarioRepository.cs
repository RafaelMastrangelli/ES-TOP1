using ESTop1.Domain;
using ESTop1.Domain.Interfaces;
using ESTop1.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ESTop1.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        return await _context.Usuarios
            .Include(u => u.Assinaturas.Where(a => a.Status == StatusAssinatura.Ativa))
            .Include(u => u.Time)
            .FirstOrDefaultAsync(u => u.Email == email && u.Ativo);
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id)
    {
        return await _context.Usuarios
            .Include(u => u.Assinaturas.Where(a => a.Status == StatusAssinatura.Ativa))
            .Include(u => u.Time)
            .FirstOrDefaultAsync(u => u.Id == id && u.Ativo);
    }

    public async Task<Usuario> CriarAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<Usuario> AtualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> ExisteEmailAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email);
    }
}
