using Microsoft.AspNetCore.Mvc;
using ProyectoGestorPeluqueria.Extensions;
using ProyectoGestorPeluqueria.Models;
using ProyectoGestorPeluqueria.Repositories;

namespace ProyectoGestorPeluqueria.Controllers
{
    public class CalendarioController : Controller
    {
        private readonly IRepositoryGestorPeluqueria repo;

        public CalendarioController(IRepositoryGestorPeluqueria repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> Gestionar(int id)
        {
            var usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null) return RedirectToAction("Login", "Login");
            if (usuario.RolId != 1 && usuario.RolId != 2) return RedirectToAction("Index", "Home");

            var peluqueria = await repo.FindPeluqueria(id);
            if (peluqueria == null) return NotFound();
            if (peluqueria.DuenoId != usuario.UsuarioId) return RedirectToAction("Index", "Home");

            ViewBag.Empleados = await repo.FindEmpleadosPeluqueria(id);
            ViewBag.Servicios = await repo.FindServiciosPeluqueria(id);
            return View(peluqueria);
        }

        public async Task<IActionResult> Reservar(int id)
        {
            var usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null) return RedirectToAction("Login", "Login");
            if (usuario.RolId != 3) return RedirectToAction("Index", "Home");

            var peluqueria = await repo.FindPeluqueria(id);
            if (peluqueria == null) return NotFound();

            ViewBag.Empleados = await repo.FindEmpleadosPeluqueria(id);
            ViewBag.Servicios = await repo.FindServiciosPeluqueria(id);
            ViewBag.ClienteId = usuario.UsuarioId;
            return View(peluqueria);
        }

        [HttpGet]
        public async Task<IActionResult> GetEventos(int peluqueriaId, string start, string end, int? empleadoId)
        {
            if (!DateTimeOffset.TryParse(start, out var startOffset) || !DateTimeOffset.TryParse(end, out var endOffset))
                return BadRequest();

            var startDate = startOffset.DateTime;
            var endDate   = endOffset.DateTime;

            var citas = await repo.GetCitasCalendario(peluqueriaId, startDate, endDate, empleadoId);
            var horarios = await repo.GetHorariosCalendario(peluqueriaId, startDate, endDate, empleadoId);

            var events = new List<object>();

            foreach (var c in citas)
            {
                string color = c.EstadoId switch
                {
                    1 => "#f59e0b",
                    2 => "#10b981",
                    3 => "#ef4444",
                    4 => "#64748b",
                    _ => "#4f46e5"
                };
                events.Add(new
                {
                    id = "cita_" + c.CitaId,
                    title = $"{c.NombreCliente ?? "Cliente"} · {c.NombreServicio ?? ""}",
                    start = c.FechaHoraInicio.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = c.FechaHoraFin.ToString("yyyy-MM-ddTHH:mm:ss"),
                    backgroundColor = color,
                    borderColor = color,
                    extendedProps = new
                    {
                        type = "cita",
                        citaId = c.CitaId,
                        empleadoId = c.EmpleadoId,
                        clienteId = c.ClienteId,
                        estadoId = c.EstadoId,
                        estado = c.NombreEstado,
                        empleado = c.NombreEmpleado,
                        servicio = c.NombreServicio,
                        notas = c.NotasCliente
                    }
                });
            }

            foreach (var h in horarios)
            {
                var nombreEmpleado = h.Empleado?.Nombre ?? "Empleado";
                events.Add(new
                {
                    id = "horario_" + h.HorarioId,
                    title = $"Disponible · {nombreEmpleado}",
                    start = h.FechaHoraApertura.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = h.FechaHoraCierre.ToString("yyyy-MM-ddTHH:mm:ss"),
                    display = "background",
                    backgroundColor = "#bbf7d0",
                    extendedProps = new
                    {
                        type = "horario",
                        empleadoId = h.EmpleadoId,
                        empleadoNombre = nombreEmpleado,
                        horarioId = h.HorarioId
                    }
                });
            }

            return Json(events);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmpleadosDisponibles(int peluqueriaId, string inicio, int servicioId)
        {
            if (!DateTime.TryParse(inicio, out var inicioDate))
                return BadRequest();

            var servicio = await repo.FindServicio(servicioId);
            if (servicio == null)
                return BadRequest();

            var fin = inicioDate.AddMinutes(servicio.DuracionMin);
            var empleados = await repo.FindEmpleadosPeluqueria(peluqueriaId);

            var disponibles = new List<object>();
            foreach (var emp in empleados)
            {
                if (await repo.EmpleadoDisponible(emp.EmpleadoId, inicioDate, fin))
                {
                    disponibles.Add(new { empleadoId = emp.EmpleadoId, nombre = emp.Nombre });
                }
            }

            return Json(disponibles);
        }

        [HttpPost]
        public async Task<IActionResult> CrearCita([FromBody] CrearCitaRequest req)
        {
            var usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null) return Unauthorized();

            try
            {
                await repo.CrearCita(usuario.UsuarioId, req.EmpleadoId, req.ServicioId, req.Inicio, req.Notas);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoRequest req)
        {
            var usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null) return Unauthorized();

            await repo.CambiarEstadoCita(req.CitaId, req.EstadoId);
            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AgregarHorario([FromBody] AgregarHorarioRequest req)
        {
            var usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null) return Unauthorized();
            if (usuario.RolId != 1 && usuario.RolId != 2) return Forbid();

            await repo.AgregarHorario(req.EmpleadoId, req.Apertura, req.Cierre);
            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AgregarServicio([FromBody] AgregarServicioRequest req)
        {
            var usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null) return Unauthorized();
            if (usuario.RolId != 1 && usuario.RolId != 2) return Forbid();

            try
            {
                await repo.CreateServicioPeluqueria(req.Nombre, req.Precio, req.DuracionMin, req.PeluqueriaId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> AgregarEmpleado([FromBody] AgregarEmpleadoRequest req)
        {
            var usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null) return Unauthorized();
            if (usuario.RolId != 1 && usuario.RolId != 2) return Forbid();

            try
            {
                await repo.CreateEmpleadoPeluqueria(req.Nombre, req.PeluqueriaId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> EliminarHorario([FromBody] EliminarIdRequest req)
        {
            var usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null) return Unauthorized();
            if (usuario.RolId != 1 && usuario.RolId != 2) return Forbid();

            try
            {
                await repo.DeleteHorarioAsync(req.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> EliminarServicio([FromBody] EliminarIdRequest req)
        {
            var usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null) return Unauthorized();
            if (usuario.RolId != 1 && usuario.RolId != 2) return Forbid();

            try
            {
                await repo.DeleteServicioAsync(req.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            return Ok(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> EliminarEmpleado([FromBody] EliminarIdRequest req)
        {
            var usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null) return Unauthorized();
            if (usuario.RolId != 1 && usuario.RolId != 2) return Forbid();

            try
            {
                await repo.DeleteEmpleadoAsync(req.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            return Ok(new { success = true });
        }
    }

    public record CrearCitaRequest(int EmpleadoId, int ServicioId, DateTime Inicio, string? Notas);
    public record CambiarEstadoRequest(int CitaId, int EstadoId);
    public record AgregarHorarioRequest(int EmpleadoId, DateTime Apertura, DateTime Cierre);
    public record AgregarServicioRequest(string Nombre, decimal Precio, int DuracionMin, int PeluqueriaId);
    public record AgregarEmpleadoRequest(string Nombre, int PeluqueriaId);
    public record EliminarIdRequest(int Id);
}
