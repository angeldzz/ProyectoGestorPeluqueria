using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using ProyectoGestorPeluqueria.Models;
using ProyectoGestorPeluqueria.Repositories;
using System.Security.Claims;

namespace ProyectoGestorPeluqueria.Policies
{
    public class TienePeluqueriasRequirement :
        AuthorizationHandler<TienePeluqueriasRequirement>, IAuthorizationRequirement
    {
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, TienePeluqueriasRequirement requirement)
        {
            var mvcContext = context.Resource as AuthorizationFilterContext;
            IRepositoryUsuarios repo = (IRepositoryUsuarios)mvcContext!.HttpContext.RequestServices
                .GetService(typeof(IRepositoryUsuarios))!;

            int usuarioId = int.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            List<Peluqueria> peluquerias = await repo.GetPeluqueriasUsuarioAsync(usuarioId);

            if (peluquerias.Count > 0)
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
