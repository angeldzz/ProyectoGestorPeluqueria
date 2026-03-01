using Microsoft.AspNetCore.Mvc;
using ProyectoGestorPeluqueria.Extensions;
using ProyectoGestorPeluqueria.Models;
using ProyectoGestorPeluqueria.Repositories;

namespace ProyectoGestorPeluqueria.Controllers
{
    public class GestionController : Controller
    {
        private IRepositoryGestion repo;
        private IRepositoryUsuarios repoUser;
        public GestionController(IRepositoryGestion repo, IRepositoryUsuarios repoUser)
        {
            this.repo = repo;
            this.repoUser = repoUser;
        }
        public async Task<IActionResult> DetailsPeluqueria(int id)
        {
            VwPeluqueriaDuenoServicio? peluqueria = await this.repo.FindPeluqueria(id);
            if (peluqueria == null)
            {
                return NotFound();
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
            return RedirectToAction("Index", "Home");
        }
    }
}
