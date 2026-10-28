using Lab11.Models;
using Lab11.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Lab11.Controllers;

public class AsistentesController : Controller
{
    private readonly AsistenteRepositorio _asistenteRepositorio;

    public AsistentesController(AsistenteRepositorio asistenteRepositorio)
    {
        _asistenteRepositorio = asistenteRepositorio;
    }

    public async Task<IActionResult> Index()
    {
        var asistentes = await _asistenteRepositorio.ObtenerActivosAsync();
        return View(asistentes);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Asistente asistente)
    {
        if (await _asistenteRepositorio.ExisteDniAsync(asistente.Dni))
        {
            ModelState.AddModelError(nameof(Asistente.Dni), "Ya existe un asistente registrado con ese DNI.");
        }

        if (await _asistenteRepositorio.ExisteCorreoAsync(asistente.Correo))
        {
            ModelState.AddModelError(nameof(Asistente.Correo), "Ya existe un asistente registrado con ese correo.");
        }

        if (ModelState.IsValid)
        {
            await _asistenteRepositorio.AgregarAsync(asistente);
            TempData["Mensaje"] = "Asistente registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        return View(asistente);
    }
}
