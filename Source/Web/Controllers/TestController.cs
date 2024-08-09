using EntityStorageServices;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("test")]
public class TestController(TransportCompanyContext dbContext) : Controller
{
    [Route("load-test-data-into-the-database")]
    public async Task Zxc()
    {
        await TransportCompanyContext.LoadTestData(dbContext);
    }
}