using Microsoft.AspNetCore.Mvc;
using ProyectoGestorPeluqueria.Extensions;
using ProyectoGestorPeluqueria.Filters;
using ProyectoGestorPeluqueria.Models;
using ProyectoGestorPeluqueria.Repositories;
using System.Security.Claims;

namespace ProyectoGestorPeluqueria.Controllers
{
    public class GestionController : Controller
    {
        private IRepositoryGestorPeluqueria repo;
        private IRepositoryUsuarios repoUser;
        public GestionController(IRepositoryGestorPeluqueria repo, IRepositoryUsuarios repoUser)
        {
            this.repo = repo;
            this.repoUser = repoUser;
        }
        public async Task<IActionResult> DetailsPeluqueria(int id, bool delete)
        {
            VwPeluqueriaDuenoServicio? peluqueria = await this.repo.FindPeluqueria(id);
            if (peluqueria == null)
            {
                return NotFound();
            }
        if (delete)
        {
            if (!User.Identity!.IsAuthenticated || (!User.IsInRole("1") && !User.IsInRole("2")))
                return RedirectToAction("ErrorAcceso", "Managed");

            await this.repo.DeletePeluqueriaAsync(id);

                var peluquerias = HttpContext.Session.GetObject<List<Peluqueria>>("peluqueriasUsuario");
                if (peluquerias != null)
                {
                    peluquerias.RemoveAll(p => p.PeluqueriaId == id);
                    HttpContext.Session.SetObject("peluqueriasUsuario", peluquerias);
                }

                TempData["SwalSuccess"] = $"La peluquería '{peluqueria.NombrePeluqueria}' ha sido eliminada correctamente.";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Empleados = await this.repo.FindEmpleadosPeluqueria(id);
            ViewBag.Servicios = await this.repo.FindServiciosPeluqueria(id);
            return View(peluqueria);
        }
        [AuthorizeUsuarios("2")]
        public IActionResult RegistrarNegocio()
        {
            return View();
        }

        [HttpPost]
        [AuthorizeUsuarios("2")]
        public async Task<IActionResult> RegistrarNegocio
            (string nombre, string? direccion,
            string? urlLogo, string? cordenadas)
        {
            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await this.repo.CreatePeluqueria(nombre, direccion, urlLogo, cordenadas, usuarioId);
            List<Peluqueria> peluquerias = await this.repoUser.GetPeluqueriasUsuarioAsync(usuarioId);
            HttpContext.Session.SetObject("peluqueriasUsuario", peluquerias);
            int nuevaId = peluquerias.Max(p => p.PeluqueriaId);
            return RedirectToAction("Gestionar", "Calendario", new { id = nuevaId });
        }
        [AuthorizeUsuarios("3")]
        public async Task<IActionResult> CitasUsuario()
        {
            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var citas = await this.repo.GetCitasClienteAsync(
                usuarioId,
                DateTime.Today.AddYears(-2),
                DateTime.Today.AddYears(2));
            return View(citas);
        }

        [HttpGet]
        [AuthorizeUsuarios("3")]
        public async Task<IActionResult> GetEventosCliente(string start, string end)
        {
            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (!DateTimeOffset.TryParse(start, out var startOffset) ||
                !DateTimeOffset.TryParse(end,   out var endOffset))
                return BadRequest();

            var citas = await this.repo.GetCitasClienteAsync(
                usuarioId, startOffset.DateTime, endOffset.DateTime);

            var events = citas.Select(c => new
            {
                id    = "cita_" + c.CitaId,
                title = c.NombreServicio ?? "Cita",
                start = c.FechaHoraInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                end   = c.FechaHoraFin.ToString("yyyy-MM-ddTHH:mm:ss"),
                backgroundColor = c.EstadoId switch
                {
                    1 => "#f59e0b",
                    2 => "#10b981",
                    3 => "#ef4444",
                    4 => "#64748b",
                    _ => "#4f46e5"
                },
                borderColor = c.EstadoId switch
                {
                    1 => "#f59e0b",
                    2 => "#10b981",
                    3 => "#ef4444",
                    4 => "#64748b",
                    _ => "#4f46e5"
                },
                extendedProps = new
                {
                    citaId      = c.CitaId,
                    empleado    = c.NombreEmpleado,
                    servicio    = c.NombreServicio,
                    estado      = c.NombreEstado,
                    estadoId    = c.EstadoId,
                    notas       = c.NotasCliente,
                    peluqueriaId = c.PeluqueriaId,
                    inicio      = c.FechaHoraInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                    fin         = c.FechaHoraFin.ToString("yyyy-MM-ddTHH:mm:ss")
                }
            });

            return Json(events);
        }
    }
}
