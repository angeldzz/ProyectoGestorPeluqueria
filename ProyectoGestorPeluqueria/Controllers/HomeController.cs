using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProyectoGestorPeluqueria.Models;
using ProyectoGestorPeluqueria.Repositories;

namespace ProyectoGestorPeluqueria.Controllers
{
    public class HomeController : Controller
    {
        private IRepositoryGestion repo;
        public HomeController(IRepositoryGestion repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index(string? buscar = null, int pagina = 1)
        {
            const int tamanoPagina = 3;

            List<VwPeluqueriaDuenoServicio> peluquerias = await this.repo.MostrarPeluquerias();

            var todas = peluquerias
                .GroupBy(p => p.PeluqueriaId)
                .Select(g => g.First())
                .ToList();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                todas = todas
                    .Where(p => p.NombrePeluqueria.Contains(buscar, StringComparison.OrdinalIgnoreCase)
                             || (p.Direccion != null && p.Direccion.Contains(buscar, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            int totalPaginas = (int)Math.Ceiling(todas.Count / (double)tamanoPagina);
            pagina = Math.Clamp(pagina, 1, totalPaginas == 0 ? 1 : totalPaginas);

            var paginadas = todas
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalResultados = todas.Count;
            ViewBag.Buscar = buscar;

            return View(paginadas);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
