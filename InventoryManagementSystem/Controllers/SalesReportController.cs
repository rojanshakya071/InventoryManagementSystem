using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    public class SalesReportController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
