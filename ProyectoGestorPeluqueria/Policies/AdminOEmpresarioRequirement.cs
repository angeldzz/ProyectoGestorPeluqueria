using Microsoft.AspNetCore.Authorization;

namespace ProyectoGestorPeluqueria.Policies
{
    public class AdminOEmpresarioRequirement :
        AuthorizationHandler<AdminOEmpresarioRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, AdminOEmpresarioRequirement requirement)
        {
            if (context.User.IsInRole("1") || context.User.IsInRole("2"))
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}
