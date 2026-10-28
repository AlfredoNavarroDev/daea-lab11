using Lab11.Data;
using Lab11.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab11.Repositorios;

public class InscripcionRepositorio
{
    private readonly EventosContext _context;

    public InscripcionRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<Inscripcion>> ObtenerTodasAsync()
    {
        return await _context.Inscripciones
            .AsNoTracking()
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .OrderByDescending(i => i.FechaInscripcion)
            .ToListAsync();
    }

    public async Task<Inscripcion?> ObtenerPorIdAsync(int id)
    {
        return await _context.Inscripciones.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<List<Evento>> ObtenerEventosActivosAsync()
    {
        return await _context.Eventos
            .AsNoTracking()
            .Where(e => e.Activo)
            .OrderBy(e => e.Fecha)
            .ToListAsync();
    }

    public async Task<List<Asistente>> ObtenerAsistentesActivosAsync()
    {
        return await _context.Asistentes
            .AsNoTracking()
            .Where(a => a.Activo)
            .OrderBy(a => a.Nombres)
            .ToListAsync();
    }

    public async Task<Evento?> ObtenerEventoPorIdAsync(int eventoId)
    {
        return await _context.Eventos.AsNoTracking().FirstOrDefaultAsync(e => e.Id == eventoId);
    }

    public async Task<bool> AsistenteYaInscritoAsync(int eventoId, int asistenteId)
    {
        return await _context.Inscripciones.AnyAsync(i =>
            i.EventoId == eventoId && i.AsistenteId == asistenteId && i.Estado == "Confirmada");
    }

    public async Task<int> ContarInscritosConfirmadosAsync(int eventoId)
    {
        return await _context.Inscripciones.CountAsync(i =>
            i.EventoId == eventoId && i.Estado == "Confirmada");
    }

    public async Task<int> ContarConfirmadasAsync()
    {
        return await _context.Inscripciones.CountAsync(i => i.Estado == "Confirmada");
    }

    public async Task AgregarAsync(Inscripcion inscripcion)
    {
        _context.Inscripciones.Add(inscripcion);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GuardarCambiosAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
