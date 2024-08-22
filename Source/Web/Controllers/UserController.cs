using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("user")]
public class UserController : Controller
{
    [HttpGet]
    [Route("auth")]
    public IActionResult GetAuthPage()
    {
        return View("Auth");
    }
}