using Lab11.Models;
using Lab11.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab11.Controllers;

public class EventosController : Controller
{
    private readonly EventoRepositorio _eventoRepositorio;

    public EventosController(EventoRepositorio eventoRepositorio)
    {
        _eventoRepositorio = eventoRepositorio;
    }

    public async Task<IActionResult> Index(int? categoriaId)
    {
        var eventos = await _eventoRepositorio.ObtenerActivosAsync(categoriaId);
        ViewBag.Categorias = new SelectList(await _eventoRepositorio.ObtenerCategoriasActivasAsync(), "Id", "Nombre", categoriaId);
        return View(eventos);
    }

    public async Task<IActionResult> Details(int id)
    {
        var evento = await _eventoRepositorio.ObtenerDetalleAsync(id);
        if (evento == null)
        {
            return NotFound();
        }
        return View(evento);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Categorias = new SelectList(await _eventoRepositorio.ObtenerCategoriasActivasAsync(), "Id", "Nombre");
        ViewBag.Modalidades = new SelectList(_eventoRepositorio.ObtenerModalidades());
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Evento evento)
    {
        if (ModelState.IsValid)
        {
            await _eventoRepositorio.AgregarAsync(evento);
            TempData["Mensaje"] = "Evento creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categorias = new SelectList(await _eventoRepositorio.ObtenerCategoriasActivasAsync(), "Id", "Nombre", evento.CategoriaId);
        ViewBag.Modalidades = new SelectList(_eventoRepositorio.ObtenerModalidades(), evento.Modalidad);
        return View(evento);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var evento = await _eventoRepositorio.ObtenerPorIdAsync(id);
        if (evento == null)
        {
            return NotFound();
        }
        ViewBag.Categorias = new SelectList(await _eventoRepositorio.ObtenerCategoriasActivasAsync(), "Id", "Nombre", evento.CategoriaId);
        ViewBag.Modalidades = new SelectList(_eventoRepositorio.ObtenerModalidades(), evento.Modalidad);
        return View(evento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Evento evento)
    {
        if (id != evento.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var eventoDb = await _eventoRepositorio.ObtenerPorIdAsync(id);
            if (eventoDb == null)
            {
                return NotFound();
            }

            eventoDb.Titulo = evento.Titulo;
            eventoDb.Fecha = evento.Fecha;
            eventoDb.Lugar = evento.Lugar;
            eventoDb.Aforo = evento.Aforo;
            eventoDb.Precio = evento.Precio;
            eventoDb.CategoriaId = evento.CategoriaId;
            eventoDb.Modalidad = evento.Modalidad;

            await _eventoRepositorio.GuardarCambiosAsync();
            TempData["Mensaje"] = "Evento actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Categorias = new SelectList(await _eventoRepositorio.ObtenerCategoriasActivasAsync(), "Id", "Nombre", evento.CategoriaId);
        ViewBag.Modalidades = new SelectList(_eventoRepositorio.ObtenerModalidades(), evento.Modalidad);
        return View(evento);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var evento = await _eventoRepositorio.ObtenerDetalleAsync(id);
        if (evento == null)
        {
            return NotFound();
        }
        return View(evento);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var evento = await _eventoRepositorio.ObtenerPorIdAsync(id);
        if (evento == null)
        {
            return NotFound();
        }

        evento.Activo = false;
        await _eventoRepositorio.GuardarCambiosAsync();
        TempData["Mensaje"] = "Evento eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
