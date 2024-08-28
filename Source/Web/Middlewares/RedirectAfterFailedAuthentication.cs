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
            
            // Это имело бы смысл, если на сайте была бы отдельная страница с формой для ввода данных для входа в админку, но у меня вход происходит просто с помощью правильного URL
            // if (requiredRoles.Any(r => r.AllowedRoles.Contains("Administrator")))
            //     context.Response.Redirect("/admin/login");
            if (requiredRoles.Any(r => r.AllowedRoles.Contains("User")))
                context.Response.Redirect("/user/auth");
        }
    }
    
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
}