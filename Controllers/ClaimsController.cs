using Microsoft.AspNetCore.Mvc;

namespace UserManagementApi.Controllers
{
    public class ClaimsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
