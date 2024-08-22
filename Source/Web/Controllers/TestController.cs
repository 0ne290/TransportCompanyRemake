using Application;
using Application.Actors;
using Domain.Entities;
using EntityStorageServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Web.Controllers;

[Route("test")]
public class TestController(TransportCompanyContext dbContext, Administrator administrator) : Controller
{
    [Route("load-test-data-into-the-database")]
    public async Task Zxc()
    {
        await TransportCompanyContext.LoadTestData(dbContext);
    }

    [Route("clear-database")]
    public async Task Cxz()
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [HttpPost]
    [Route("drivers")]
    public async Task CreateDrivers(
        [FromBody] IReadOnlyCollection<Application.Dtos.Driver.CreateRequest> createRequests)
    {
        await administrator.CreateDrivers(createRequests);
    }

    [HttpPatch]
    [Route("drivers")]
    public async Task UpdateDrivers(
        [FromBody] IReadOnlyCollection<Application.Dtos.Driver.UpdateRequest> updateRequests)
    {
        Console.WriteLine(JsonConvert.SerializeObject(updateRequests));
        await administrator.UpdateDrivers(updateRequests);
    }

    [HttpDelete]
    [Route("drivers")]
    public async Task DeleteDrivers(string filter)
    {
        await administrator.DeleteDrivers(FilterParser.Parse<Driver>(filter));
    }
    
    [HttpPost]
    [Route("trucks")]
    public async Task CreateTrucks(
        [FromBody] IReadOnlyCollection<Application.Dtos.Truck.CreateRequest> createRequests)
    {
        await administrator.CreateTrucks(createRequests);
    }

    [HttpPatch]
    [Route("trucks")]
    public async Task UpdateTrucks([FromBody] IReadOnlyCollection<Application.Dtos.Truck.UpdateRequest> updateRequests)
    {
        Console.WriteLine(JsonConvert.SerializeObject(updateRequests));
        await administrator.UpdateTrucks(updateRequests);
    }
    
    [HttpDelete]
    [Route("trucks")]
    public async Task DeleteTrucks(string filter)
    {
        await administrator.DeleteTrucks(FilterParser.Parse<Truck>(filter));
    }

    [HttpGet]
    [Route("branches")]
    public async Task<IActionResult> GetAdminPage()
    {
        return View("Branches",
            (await administrator.GetBranches(FilterParser.Parse<Branch>("true"), true, true))
            .Select(b => new ViewModels.Branch(b)).ToList());
    }

    [HttpPost]
    [Route("branches")]
    public async Task CreateBranches(
        [FromBody] IReadOnlyCollection<Application.Dtos.Branch.CreateRequest> createRequests)
    {
        await administrator.CreateBranches(createRequests);
    }

    [HttpPatch]
    [Route("branches")]
    public async Task UpdateBranches(
        [FromBody] IReadOnlyCollection<Application.Dtos.Branch.UpdateRequest> updateRequests)
    {
        Console.WriteLine(JsonConvert.SerializeObject(updateRequests));
        await administrator.UpdateBranches(updateRequests);
    }

    [HttpDelete]
    [Route("branches")]
    public async Task DeleteBranches(string filter)
    {
        await administrator.DeleteBranches(FilterParser.Parse<Branch>(filter));
    }
}