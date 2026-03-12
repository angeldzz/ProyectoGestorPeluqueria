using Microsoft.AspNetCore.Authorization;

namespace ProyectoGestorPeluqueria.Policies
{
    public class EmpresarioRequirement :
        AuthorizationHandler<EmpresarioRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, EmpresarioRequirement requirement)
        {
            if (context.User.IsInRole("2"))
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}
