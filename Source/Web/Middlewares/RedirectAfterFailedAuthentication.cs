using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;

namespace Web.Middlewares;

public class RedirectAfterFailedAuthentication : IAuthorizationMiddlewareResultHandler
{
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        
        if (!authorizeResult.Succeeded)
        {
            var requiredRoles = policy.Requirements.OfType<RolesAuthorizationRequirement>().ToList();
            
            if (requiredRoles.Any(r => r.AllowedRoles.Contains("Administrator")))
                context.Response.Redirect("/admin/login");
            else if (requiredRoles.Any(r => r.AllowedRoles.Contains("User")))
                context.Response.Redirect("/user/auth");
        }
    }
    
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
}