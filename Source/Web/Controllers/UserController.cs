using System.Security.Claims;
using Application;
using Application.Actors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Dtos;
using Order = Domain.Entities.Order;

namespace Web.Controllers;

[Route("user")]
public class UserController(User userActor, Administrator administrator) : Controller
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
    public async Task<IActionResult> GetOrdersPage()
    {
        return View("Orders", (await administrator.GetOrders(FilterParser.Parse<Order>("true")))
            .Select(o => new Dtos.Order(o)).ToList());
    }
    
    [Authorize(Roles = "User")]
    [HttpGet]
    [Route("create-order")]
    public IActionResult GetOrderCreationPage()
    {
        return View("CreateOrder");
    }
    
    [Authorize(Roles = "User")]
    [HttpPost]
    [Route("create-order")]
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