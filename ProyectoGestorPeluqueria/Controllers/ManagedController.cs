using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ProyectoGestorPeluqueria.Extensions;
using ProyectoGestorPeluqueria.Models;
using ProyectoGestorPeluqueria.Repositories;
using System.Security.Claims;

namespace ProyectoGestorPeluqueria.Controllers
{
    public class ManagedController : Controller
    {
        private readonly IRepositoryUsuarios repo;

        public ManagedController(IRepositoryUsuarios repo)
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
                ViewData["MENSAJE"] = "Email o contraseña incorrectos.";
                return View();
            }

            ClaimsIdentity identity = new ClaimsIdentity(
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role);

            identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Nombre));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, usuario.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, usuario.RolId.ToString()!));
            if (usuario.Telefono != null)
                identity.AddClaim(new Claim("Telefono", usuario.Telefono));

            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);

            List<Peluqueria> peluquerias = await this.repo.GetPeluqueriasUsuarioAsync(usuario.UsuarioId);
            HttpContext.Session.SetObject("peluqueriasUsuario", peluquerias);

            if (TempData["controller"] != null && TempData["action"] != null)
            {
                string controller = TempData["controller"]!.ToString()!;
                string action = TempData["action"]!.ToString()!;
                if (TempData["id"] != null)
                {
                    string id = TempData["id"]!.ToString()!;
                    return RedirectToAction(action, controller, new { id });
                }
                return RedirectToAction(action, controller);
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
