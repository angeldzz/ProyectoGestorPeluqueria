using Microsoft.AspNetCore.Mvc;

namespace ProyectoGestorPeluqueria.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Registrarse()
        {
            return View();
        }
    }
}
