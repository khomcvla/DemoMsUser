using DemoMsUser.Common.Constants;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DemoMsUser.Common.Handlers
{
    public class AdminOnlyOrSubIdMatchRequirement : IAuthorizationRequirement { }

    public class AdminOnlyOrSubIdMatchHandler : AuthorizationHandler<SubIdMatchRequirement>
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

            // is admin
            if (context.User.HasClaim(c => (c.Type == "role" && c.Value == UserRoles.Admin)))
                context.Succeed(requirement);

            // sub and id are same
            if (idFromPath.ToString() == subFromToken)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
