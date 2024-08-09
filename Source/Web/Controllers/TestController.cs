using Application.Actors;
using EntityStorageServices;
using Microsoft.AspNetCore.Mvc;

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
    public async Task CreateDrivers(IReadOnlyCollection<Application.Dtos.Driver.CreateRequest> createRequests)
    {
        await administrator.CreateDrivers(createRequests);
    }
    
    [HttpPost]
    [Route("create-branches")]
    public async Task CreateBranches([FromBody]IReadOnlyCollection<Application.Dtos.Branch.CreateRequest> createRequests)
    {
        Console.WriteLine(createRequests.Count);
        await administrator.CreateBranches(createRequests);
    }
}