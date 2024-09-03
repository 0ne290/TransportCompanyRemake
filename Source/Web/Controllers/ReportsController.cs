using Application.Actors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Authorize(Roles = "Administrator")]
[Route("reports")]
public class ReportsController(Administrator administrator) : Controller
{
    [HttpGet]
    [Route("")]
    public IActionResult Index()
    {
        return View("Index");
    }
    
    [HttpGet]
    [Route("clients-by-profit")]
    public async Task<IActionResult> ClientsByProfit()
    {
        return View("ClientsByProfit", await administrator.AggregateAllUsersByProfit());
    }
}