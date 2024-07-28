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

    // Выдавать полностью загруженные филиалы (т. е. вместе со всеми связанными сущностями - водителями и фурами). На фронтенде будет специальный эндпоинт для администрирования филиалов и все эти полученные филиалы будут хранится там в переменной и использоваться во всех сценариях без подзагрузки дополнительных данных. В этом и отличие от моей стандартной методики - на фронтенд сразу же отправляются все нужные данные одной пачкой и дальнейшее общение между фронтендом и бэкендом не происходит
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
