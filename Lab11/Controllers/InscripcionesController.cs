using Lab11.Models;
using Lab11.Repositorios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab11.Controllers;

public class InscripcionesController : Controller
{
    private readonly InscripcionRepositorio _inscripcionRepositorio;

    public InscripcionesController(InscripcionRepositorio inscripcionRepositorio)
    {
        _inscripcionRepositorio = inscripcionRepositorio;
    }

    public async Task<IActionResult> Index()
    {
        var inscripciones = await _inscripcionRepositorio.ObtenerTodasAsync();
        return View(inscripciones);
    }

    public async Task<IActionResult> Create()
    {
        await CargarListasAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inscripcion inscripcion)
    {
        var evento = await _inscripcionRepositorio.ObtenerEventoPorIdAsync(inscripcion.EventoId);

        if (evento == null)
        {
            ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento seleccionado no existe.");
        }
        else
        {
            if (evento.Fecha < DateTime.Now)
            {
                ModelState.AddModelError(nameof(Inscripcion.EventoId), "El evento ya pasó y no admite inscripciones.");
            }

            if (await _inscripcionRepositorio.AsistenteYaInscritoAsync(inscripcion.EventoId, inscripcion.AsistenteId))
            {
                ModelState.AddModelError(nameof(Inscripcion.AsistenteId), "El asistente ya está inscrito en este evento.");
            }

            var confirmados = await _inscripcionRepositorio.ContarInscritosConfirmadosAsync(inscripcion.EventoId);
            if (confirmados >= evento.Aforo)
            {
                ModelState.AddModelError(nameof(Inscripcion.EventoId), "El aforo del evento está completo.");
            }
        }

        ModelState.Remove(nameof(Inscripcion.Estado));
        ModelState.Remove(nameof(Inscripcion.FechaInscripcion));

        if (ModelState.IsValid)
        {
            inscripcion.FechaInscripcion = DateTime.Now;
            inscripcion.Estado = "Confirmada";
            await _inscripcionRepositorio.AgregarAsync(inscripcion);
            TempData["Mensaje"] = "Inscripción registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        await CargarListasAsync();
        return View(inscripcion);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancelar(int id)
    {
        var inscripcion = await _inscripcionRepositorio.ObtenerPorIdAsync(id);
        if (inscripcion == null)
        {
            return NotFound();
        }

        inscripcion.Estado = "Cancelada";
        await _inscripcionRepositorio.GuardarCambiosAsync();
        TempData["Mensaje"] = "Inscripción cancelada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarListasAsync()
    {
        ViewBag.Eventos = new SelectList(await _inscripcionRepositorio.ObtenerEventosActivosAsync(), "Id", "Titulo");
        ViewBag.Asistentes = new SelectList(await _inscripcionRepositorio.ObtenerAsistentesActivosAsync(), "Id", "Nombres");
    }
}
