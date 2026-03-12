using Microsoft.AspNetCore.Mvc;
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
            return RedirectToAction("Login", "Managed");
        }
    }
}


