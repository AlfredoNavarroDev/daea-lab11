using Lab11.Data;
using Lab11.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab11.Repositorios;

public class AsistenteRepositorio
{
    private readonly EventosContext _context;

    public AsistenteRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<Asistente>> ObtenerActivosAsync()
    {
        return await _context.Asistentes
            .AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();
    }

    public async Task<bool> ExisteDniAsync(string dni)
    {
        return await _context.Asistentes.AnyAsync(a => a.Dni == dni);
    }

    public async Task<bool> ExisteCorreoAsync(string correo)
    {
        return await _context.Asistentes.AnyAsync(a => a.Correo == correo);
    }

    public async Task AgregarAsync(Asistente asistente)
    {
        _context.Asistentes.Add(asistente);
        await _context.SaveChangesAsync();
    }
}
