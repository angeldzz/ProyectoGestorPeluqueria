using Microsoft.AspNetCore.Mvc;
using ProyectoGestorPeluqueria.Extensions;
using ProyectoGestorPeluqueria.Models;
using ProyectoGestorPeluqueria.Repositories;

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
        public IActionResult RegistrarNegocio()
        {
            Usuario? usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else if (usuario.RolId != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarNegocio
            (string nombre, string? direccion,
            string? urlLogo, string? cordenadas)
        {
            Usuario? usuario = HttpContext.Session.GetObject<Usuario>("usuario");
            if (usuario == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else if(usuario.RolId != 2)
            {
                return RedirectToAction("Index", "Home");
            }

            await this.repo.CreatePeluqueria(nombre, direccion, urlLogo, cordenadas, usuario.UsuarioId);
            List<Peluqueria> peluquerias = await this.repoUser.GetPeluqueriasUsuarioAsync(usuario.UsuarioId);
            HttpContext.Session.SetObject("peluqueriasUsuario", peluquerias);
            int nuevaId = peluquerias.Max(p => p.PeluqueriaId);
            return RedirectToAction("Gestionar", "Calendario", new { id = nuevaId });
        }
    }
}
