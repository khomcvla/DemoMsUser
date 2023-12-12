using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DemoMsUser.Common.Handlers
{
    public class SubIdMatchRequirement : IAuthorizationRequirement { }

    public class SubIdMatchHandler : AuthorizationHandler<SubIdMatchRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SubIdMatchRequirement requirement)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (requirement == null) throw new ArgumentNullException(nameof(requirement));

            var authFilterCtx = (AuthorizationFilterContext)context.Resource;
            var httpContext = authFilterCtx.HttpContext;
            var pathData = httpContext.Request.Path.Value.Split("/");

            long idFromPath = long.Parse(pathData[pathData.Length - 1]);
            var subFromToken = httpContext.User.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject)?.Value;

            if (idFromPath.ToString() == subFromToken)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
