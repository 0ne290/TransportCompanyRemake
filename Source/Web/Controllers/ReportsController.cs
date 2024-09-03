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
    
    [HttpGet]
    [Route("drivers-by-profit")]
    public async Task<IActionResult> DriversByProfit()
    {
        return View("DriversByProfit", await administrator.AggregateAllDriversByProfit());
    }
    
    [HttpGet]
    [Route("drivers-by-weekly-workload")]
    public async Task<IActionResult> DriversByWeeklyWorload()
    {
        return View("DriversByWorkload", await administrator.AggregateAllDriversByWeeklyWorkload());
    }
    
    [HttpGet]
    [Route("drivers-by-total-workload")]
    public async Task<IActionResult> DriversByTotalWorload()
    {
        return View("DriversByWorkload", await administrator.AggregateAllDriversByTotalWorkload());
    }
    
    [HttpGet]
    [Route("drivers-by-efficiency")]
    public async Task<IActionResult> DriversByEfficiency()
    {
        return View("DriversByEfficiency", await administrator.AggregateAllDriversByEfficiency());
    }
    
    [HttpGet]
    [Route("trucks-by-route-traveled")]
    public async Task<IActionResult> TrucksByRouteTraveled()
    {
        return View("TrucksByRouteTraveled", await administrator.AggregateAllTrucksByRouteTraveled());
    }
    
    [HttpGet]
    [Route("trucks-by-efficiency")]
    public async Task<IActionResult> TrucksByEfficiency()
    {
        return View("TrucksByEfficiency", await administrator.AggregateAllTrucksByEfficiency());
    }
}