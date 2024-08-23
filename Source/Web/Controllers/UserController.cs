using System.Security.Claims;
using Application.Actors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("user")]
public class UserController(User userActor) : Controller
{
    [HttpGet]
    [Route("auth")]
    public IActionResult GetAuthPage()
    {
        return View("Auth");
    }
    
    [HttpGet]
    [Route("vk-login")]
    public IActionResult GetVkLoginScriptPage()
    {
        return View("VkLogin");
    }
    
    [HttpPost]
    [Route("vk-login")]
    public async Task<IActionResult> VkLogin(Application.Dtos.User.CreateVkRequest createRequest)
    {
        var user = await userActor.TryCreateVkUser(createRequest);
        
        var claims = new[] { new Claim(ClaimTypes.Name, user.Guid), new Claim(ClaimTypes.Role, "User") };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
            IsPersistent = true
        };
            
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
        
        return Ok();
    }
}