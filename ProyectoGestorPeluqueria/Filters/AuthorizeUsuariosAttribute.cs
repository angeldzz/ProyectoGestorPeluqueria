using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ProyectoGestorPeluqueria.Filters
{
    public class AuthorizeUsuariosAttribute :
        AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeUsuariosAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity == null || !user.Identity.IsAuthenticated)
            {
                var routeValues = context.RouteData.Values;
                var tempDataFactory = context.HttpContext.RequestServices
                    .GetRequiredService<ITempDataDictionaryFactory>();
                var tempData = tempDataFactory.GetTempData(context.HttpContext);

                tempData["controller"] = routeValues["controller"]?.ToString();
                tempData["action"] = routeValues["action"]?.ToString();
                if (routeValues.ContainsKey("id"))
                    tempData["id"] = routeValues["id"]?.ToString();

                context.Result = new RedirectToActionResult("Login", "Managed", null);
                return;
            }

            if (_roles.Length > 0 && !_roles.Any(r => user.IsInRole(r)))
            {
                context.Result = new RedirectToActionResult("ErrorAcceso", "Managed", null);
            }
        }
    }
}
