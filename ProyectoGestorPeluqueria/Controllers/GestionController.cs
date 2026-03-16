using Microsoft.AspNetCore.Mvc;
using MvcNetCoreUtilidades.Helpers;
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
        private HelperPathProvider helper;
        public GestionController(IRepositoryGestorPeluqueria repo, IRepositoryUsuarios repoUser, HelperPathProvider helper)
        {
            this.repo = repo;
            this.repoUser = repoUser;
            this.helper = helper;
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
            IFormFile? fichero, string? cordenadas)
        {
            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            string? urlLogo = null;
            if (fichero != null && fichero.Length > 0)
            {
                string fileName = fichero.FileName;
                string path = this.helper.MapPath(fileName, Folders.Images);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await fichero.CopyToAsync(stream);
                }

                urlLogo = this.helper.MapUrlPath(fileName, Folders.Images);
            }

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
                !DateTimeOffset.TryParse(end, out var endOffset))
                return BadRequest();

            var citas = await this.repo.GetCitasClienteAsync(
                usuarioId, startOffset.DateTime, endOffset.DateTime);

            var events = citas.Select(c => new
            {
                id = "cita_" + c.CitaId,
                title = c.NombreServicio ?? "Cita",
                start = c.FechaHoraInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = c.FechaHoraFin.ToString("yyyy-MM-ddTHH:mm:ss"),
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
                    citaId = c.CitaId,
                    empleado = c.NombreEmpleado,
                    servicio = c.NombreServicio,
                    estado = c.NombreEstado,
                    estadoId = c.EstadoId,
                    notas = c.NotasCliente,
                    peluqueriaId = c.PeluqueriaId,
                    inicio = c.FechaHoraInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                    fin = c.FechaHoraFin.ToString("yyyy-MM-ddTHH:mm:ss")
                }
            });

            return Json(events);
        }

        [HttpGet]
        public async Task<IActionResult> GetMensajes(int peluqueriaId, int? usuarioChatId = null)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var peluqueria = await this.repo.FindPeluqueria(peluqueriaId);
            if (peluqueria == null)
                return NotFound();

            bool esDueno = peluqueria.DuenoId == usuarioId;
            List<Mensaje> mensajes;

            if (esDueno)
            {
                if (usuarioChatId.HasValue)
                {
                    mensajes = await this.repo.GetMensajesConversacionAsync(
                        peluqueriaId, usuarioId, usuarioChatId.Value);
                }
                else
                {
                    mensajes = await this.repo.GetMensajesPeluqueriaAsync(peluqueriaId);
                }
            }
            else
            {
                mensajes = await this.repo.GetMensajesConversacionAsync(
                    peluqueriaId, usuarioId, peluqueria.DuenoId);
            }

            return Json(mensajes.Select(m => new {
                mensajeId = m.MensajeId,
                mensaje = m.Mensaje1,
                hora = m.HoraCreacion?.ToString("HH:mm"),
                emisorId = m.UsuarioId
            }));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMensaje([FromBody] DeleteMensajeDto form)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            bool deleted = await this.repo.DeleteMensajeAsync(form.MensajeId, usuarioId);
            if (!deleted)
                return Forbid();

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensaje([FromBody] MensajeDto form)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            int usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            
            if (string.IsNullOrWhiteSpace(form.MensajeText))
                return BadRequest();
                
            // Evaluamos si el usuario actual es el dueño de la peluquería
            var peluqueria = await this.repo.FindPeluqueria(form.PeluqueriaId);
            bool esDueno = peluqueria != null && peluqueria.DuenoId == usuarioId;
            
            // Si es dueño y está contestando a un cliente en específico...
            // Por ahora, si es desde la vista Details, target será el mismo Dueño en un chat general
            // Lo dejaremos enviando su mensaje normal.
            
            await this.repo.CreateMensajeAsync(usuarioId, form.PeluqueriaId, form.MensajeText);

            return Ok(new { success = true });
        }
    }

    public class MensajeDto
    {
        public int PeluqueriaId { get; set; }
        public string? MensajeText { get; set; }
    }

    public class DeleteMensajeDto
    {
        public int MensajeId { get; set; }
    }
}
