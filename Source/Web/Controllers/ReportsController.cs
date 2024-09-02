using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("reports")]
public class ReportsController : Controller
{
    [Authorize(Roles = "Administrator")]
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index()
    {
        return View("Index");
    }
    
    [Authorize(Roles = "Administrator")]
    [HttpGet]
    [Route("clients-by-profit")]
    public async Task<IActionResult> ClientsByProfit()
    {
        return View("ClientsByProfit");
    }
}