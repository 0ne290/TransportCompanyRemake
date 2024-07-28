using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class AdministratorController : Controller
{
    public IActionResult GetLogin()
    {
        
    }
    
    public IActionResult PostLogin()
    {
        
    }
    
    [Authorize(Roles = "Administrator")]
    public IActionResult GetBranches()
    {
        
    }
    
    [Authorize(Roles = "Administrator")]
    public IActionResult GetBranch()
    {
        
    }
    
    [Authorize(Roles = "Administrator")]
    public IActionResult PostBranch()
    {
        
    }
    
    [Authorize(Roles = "Administrator")]
    public IActionResult GetDriver()
    {
        
    }
    
    [Authorize(Roles = "Administrator")]
    public IActionResult PostDriver()
    {
        
    }
    
    [Authorize(Roles = "Administrator")]
    public IActionResult GetTruck()
    {
        
    }
    
    [Authorize(Roles = "Administrator")]
    public IActionResult PostTruck()
    {
        
    }
    
    [Authorize(Roles = "Administrator")]
    public IActionResult GetOrders()
    {
        
    }
    
    [Authorize(Roles = "Administrator")]
    public IActionResult GetOrder()
    {
        
    }
    
    [Authorize(Roles = "Administrator")]
    public IActionResult GetBranchByOrder()
    {
        
    }
    
    [Authorize(Roles = "Administrator")]
    public IActionResult PostOrder()
    {
        
    }
}