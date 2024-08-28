using System.Security.Claims;
using Application;
using Application.Actors;
using Domain.Entities;
using EntityStorageServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Web.Controllers;

[Route("admin")]
public class AdminController(TransportCompanyContext dbContext, Administrator administrator) : Controller
{
    [Route("login/login=Rotartsinimda/password=VwXyZ90786")]
    public async Task<IActionResult> Login()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "Admin"), new Claim(ClaimTypes.Role, "Administrator") };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
            IsPersistent = true
        };
            
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);
        
        return RedirectToAction("GetAdministrationPage");
    }
    
    [Authorize(Roles = "Administrator")]
    [Route("load-test-data-into-the-database")]
    public async Task Zxc()
    {
        await TransportCompanyContext.LoadTestData(dbContext);
    }

    [Authorize(Roles = "Administrator")]
    [Route("clear-database")]
    public async Task Cxz()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [Route("drivers")]
    public async Task CreateDrivers(
        [FromBody] IReadOnlyCollection<Application.Dtos.Driver.CreateRequest> createRequests)
    {
        await administrator.CreateDrivers(createRequests);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPatch]
    [Route("drivers")]
    public async Task UpdateDrivers(
        [FromBody] IReadOnlyCollection<Application.Dtos.Driver.UpdateRequest> updateRequests)
    {
        await administrator.UpdateDrivers(updateRequests);
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete]
    [Route("drivers")]
    public async Task DeleteDrivers(string filter)
    {
        await administrator.DeleteDrivers(FilterParser.Parse<Driver>(filter));
    }
    
    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [Route("trucks")]
    public async Task CreateTrucks(
        [FromBody] IReadOnlyCollection<Application.Dtos.Truck.CreateRequest> createRequests)
    {
        await administrator.CreateTrucks(createRequests);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPatch]
    [Route("trucks")]
    public async Task UpdateTrucks([FromBody] IReadOnlyCollection<Application.Dtos.Truck.UpdateRequest> updateRequests)
    {
        await administrator.UpdateTrucks(updateRequests);
    }
    
    [Authorize(Roles = "Administrator")]
    [HttpDelete]
    [Route("trucks")]
    public async Task DeleteTrucks(string filter)
    {
        await administrator.DeleteTrucks(FilterParser.Parse<Truck>(filter));
    }

    [Authorize(Roles = "Administrator")]
    [HttpGet]
    [Route("administration")]
    public async Task<IActionResult> GetAdministrationPage()
    {
        return View("Administration",
            (await administrator.GetBranches(FilterParser.Parse<Branch>("true"), true, true))
            .Select(b => new ViewModels.Branch(b)).ToList());
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    [Route("branches")]
    public async Task CreateBranches(
        [FromBody] IReadOnlyCollection<Application.Dtos.Branch.CreateRequest> createRequests)
    {
        await administrator.CreateBranches(createRequests);
    }

    [Authorize(Roles = "Administrator")]
    [HttpPatch]
    [Route("branches")]
    public async Task UpdateBranches(
        [FromBody] IReadOnlyCollection<Application.Dtos.Branch.UpdateRequest> updateRequests)
    {
        await administrator.UpdateBranches(updateRequests);
    }

    [Authorize(Roles = "Administrator")]
    [HttpDelete]
    [Route("branches")]
    public async Task DeleteBranches(string filter)
    {
        await administrator.DeleteBranches(FilterParser.Parse<Branch>(filter));
    }
}