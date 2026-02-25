using Microsoft.AspNetCore.Mvc;

namespace ProyectoGestorPeluqueria.Controllers
{
    public class GestionController : Controller
    {
        public IActionResult RegistrarNegocio()
        {
            return View();
        }
    }
}
