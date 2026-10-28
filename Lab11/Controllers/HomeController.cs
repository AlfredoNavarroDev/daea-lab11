using Lab11.Models;
using Lab11.Repositorios;
using Lab11.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Lab11.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventoRepositorio _eventoRepositorio;
        private readonly AsistenteRepositorio _asistenteRepositorio;
        private readonly InscripcionRepositorio _inscripcionRepositorio;

        public HomeController(
            EventoRepositorio eventoRepositorio,
            AsistenteRepositorio asistenteRepositorio,
            InscripcionRepositorio inscripcionRepositorio)
        {
            _eventoRepositorio = eventoRepositorio;
            _asistenteRepositorio = asistenteRepositorio;
            _inscripcionRepositorio = inscripcionRepositorio;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new HomeDashboardViewModel
            {
                EventosActivos = (await _eventoRepositorio.ObtenerActivosAsync(null)).Count,
                AsistentesActivos = (await _asistenteRepositorio.ObtenerActivosAsync()).Count,
                InscripcionesConfirmadas = await _inscripcionRepositorio.ContarConfirmadasAsync()
            };

            return View(dashboard);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
