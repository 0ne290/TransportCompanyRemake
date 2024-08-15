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
    [Route("create-drivers")]
    public async Task CreateDrivers([FromBody]IReadOnlyCollection<Application.Dtos.Driver.CreateRequest> createRequests)
    {
        await administrator.CreateDrivers(createRequests);
    }
    
    [HttpPost]
    [Route("branches")]
    public async Task CreateBranches([FromBody]IReadOnlyCollection<Application.Dtos.Branch.CreateRequest> createRequests)
    {
        await administrator.CreateBranches(createRequests);
    }
    
    [HttpGet]
    [Route("branches")]
    public async Task<IActionResult> GetBranches()
    {
        return View("Branches", (await administrator.GetBranches(FilterParser.Parse<Branch>("true"), true, true)).Select(b => new ViewModels.Branch(b)).ToList());
    }
    
    [HttpDelete]
    [Route("branches")]
    public async Task DeleteBranches(string filter)
    {
        await administrator.DeleteBranches(FilterParser.Parse<Branch>(filter));
    }
    
    [HttpPatch]
    [Route("branches")]
    public async Task UpdateBranches([FromBody]IReadOnlyCollection<Application.Dtos.Branch.UpdateRequest> updateRequests)
    {
        Console.WriteLine(JsonConvert.SerializeObject(updateRequests));
        await administrator.UpdateBranches(updateRequests);
    }
}