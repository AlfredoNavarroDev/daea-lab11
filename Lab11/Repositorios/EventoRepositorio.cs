using Lab11.Data;
using Lab11.Models;
using Lab11.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Lab11.Repositorios;

public class EventoRepositorio
{
    private readonly EventosContext _context;

    public EventoRepositorio(EventosContext context)
    {
        _context = context;
    }

    public async Task<List<EventoListItemViewModel>> ObtenerActivosAsync(int? categoriaId)
    {
        var query = _context.Eventos.AsNoTracking().Where(e => e.Activo);

        if (categoriaId.HasValue)
        {
            query = query.Where(e => e.CategoriaId == categoriaId.Value);
        }

        return await query
            .OrderBy(e => e.Fecha)
            .Select(e => new EventoListItemViewModel
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Fecha = e.Fecha,
                Lugar = e.Lugar,
                Aforo = e.Aforo,
                Precio = e.Precio,
                Modalidad = e.Modalidad,
                CategoriaNombre = e.Categoria!.Nombre,
                InscritosConfirmados = _context.Inscripciones
                    .Count(i => i.EventoId == e.Id && i.Estado == "Confirmada")
            })
            .ToListAsync();
    }

    public async Task<Evento?> ObtenerDetalleAsync(int id)
    {
        return await _context.Eventos
            .AsNoTracking()
            .Include(e => e.Categoria)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Evento?> ObtenerPorIdAsync(int id)
    {
        return await _context.Eventos.FirstOrDefaultAsync(e => e.Id == id);
    }

    public List<string> ObtenerModalidades()
    {
        return new List<string> { "Presencial", "Virtual" };
    }

    public async Task<List<Categoria>> ObtenerCategoriasActivasAsync()
    {
        return await _context.Categorias
            .AsNoTracking()
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task AgregarAsync(Evento evento)
    {
        _context.Eventos.Add(evento);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GuardarCambiosAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
