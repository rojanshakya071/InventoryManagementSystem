using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class CurrentItemInfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
