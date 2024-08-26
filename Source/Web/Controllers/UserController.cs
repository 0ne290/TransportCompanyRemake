using System.Security.Claims;
using Application.Actors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
    
    [Authorize(Roles = "User")]
    [HttpGet]
    [Route("orders")]
    public IActionResult GetOrdersPage()
    {
        return Ok();
        //return View("Orders");
    }
    
    [HttpPost]
    [Route("vk-login")]
    public async Task<IActionResult> VkLogin([FromBody] Application.Dtos.User.CreateVkRequest createRequest)
    {
        var user = await userActor.CreateOrUpdateAndGetVkUser(createRequest);
        
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

    [HttpPost]
    [Route("standart-login")]
    public async Task<IActionResult> StandartLogin(string login, string password)
    {
        var user = await userActor.GetStandartUser(login, password);

        var claims = new[] { new Claim(ClaimTypes.Name, user.Guid), new Claim(ClaimTypes.Role, "User") };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
            IsPersistent = true
        };

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
            authProperties);

        return Ok();
    }

    [HttpPost]
    [Route("standart-register")]
    public async Task<IActionResult> StandartRegister([FromBody] Application.Dtos.User.CreateStandartRequest createRequest)
    {
        await userActor.CreateAndGetStandartUser(createRequest);
        
        return Ok();
    }
}