using System.Security.Claims;
using Application.Actors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Dtos;

namespace Web.Controllers;

public class UserController(User userActor) : Controller
{
    [HttpGet]
    [Route("user/auth")]
    public IActionResult GetAuthPage()
    {
        return View("Auth");
    }
    
    [HttpGet]
    [Route("user/vk-login")]
    public IActionResult GetVkLoginScriptPage()
    {
        return View("VkLogin");
    }

    [Authorize(Roles = "User")]
    [HttpGet]
    [Route("user/orders")]
    [Route("")]
    public async Task<IActionResult> GetOrdersPage() => View("Orders",
        (await userActor.GetOrders(HttpContext.User.FindFirst(ClaimTypes.Name)!.Value)).Select(o => new Order(o))
        .ToList());
    
    [Authorize(Roles = "User")]
    [HttpGet]
    [Route("user/create-order")]
    public IActionResult GetOrderCreationPage()
    {
        return View("CreateOrder");
    }
    
    [Authorize(Roles = "User")]
    [HttpPost]
    [Route("user/create-order")]
    public async Task<IActionResult> CreateOrder([FromBody] RequestToCreateOrder createRequest)
    {
        await userActor.CreateOrder(new Application.Dtos.Order.CreateRequest
        {
            UserGuid = HttpContext.User.FindFirst(ClaimTypes.Name)!.Value,
            StartAddress = createRequest.StartAddress,
            EndAddress = createRequest.EndAddress,
            CargoDescription = createRequest.CargoDescription,
            StartPointLatitude = createRequest.StartPointLatitude,
            StartPointLongitude = createRequest.StartPointLongitude,
            EndPointLatitude = createRequest.EndPointLatitude,
            EndPointLongitude = createRequest.EndPointLongitude,
            CargoVolume = createRequest.CargoVolume,
            CargoWeight = createRequest.CargoWeight,
            TankRequired = createRequest.TankRequired,
            HazardClassFlag = createRequest.HazardClassFlag
        });
        
        return Ok();
    }
    
    [HttpPost]
    [Route("user/vk-login")]
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
    [Route("user/standart-login")]
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
    [Route("user/standart-register")]
    public async Task<IActionResult> StandartRegister([FromBody] Application.Dtos.User.CreateStandartRequest createRequest)
    {
        await userActor.CreateAndGetStandartUser(createRequest);
        
        return Ok();
    }
}