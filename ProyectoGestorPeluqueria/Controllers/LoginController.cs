using Microsoft.AspNetCore.Mvc;
using ProyectoGestorPeluqueria.Extensions;
using ProyectoGestorPeluqueria.Models;
using ProyectoGestorPeluqueria.Repositories;

namespace ProyectoGestorPeluqueria.Controllers
{
    public class LoginController : Controller
    {
        private IRepositoryUsuarios repo;
        public LoginController(IRepositoryUsuarios repo)
        {
            this.repo = repo;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            Usuario? usuario = await this.repo.LogInUsuarioAsync(email, password);
            if (usuario == null)
            {
                ViewBag.Error = "Email o contraseña incorrectos.";
                return View();
            }
            List<Peluqueria> peluquerias = await this.repo.GetPeluqueriasUsuarioAsync(usuario.UsuarioId);
            HttpContext.Session.SetObject("usuario", usuario);
            HttpContext.Session.SetObject("peluqueriasUsuario", peluquerias);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(string nombre, string email, string? telefono,
            string password, string confirmarPassword, string role)
        {
            if (password != confirmarPassword)
            {
                ViewBag.Error = "Las contraseñas no coinciden.";
                return View();
            }

            int rolId = role == "Empresario" ? 2 : 3;
            await this.repo.CreateUsuarioAsync(nombre, password, email, telefono, rolId);
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("usuario");
            return RedirectToAction("Login");
        }
    }
}


