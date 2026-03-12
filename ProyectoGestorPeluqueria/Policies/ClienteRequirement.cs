using Microsoft.AspNetCore.Authorization;

namespace ProyectoGestorPeluqueria.Policies
{
    public class ClienteRequirement :
        AuthorizationHandler<ClienteRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, ClienteRequirement requirement)
        {
            if (context.User.IsInRole("3"))
                context.Succeed(requirement);
            else
                context.Fail();

            return Task.CompletedTask;
        }
    }
}
