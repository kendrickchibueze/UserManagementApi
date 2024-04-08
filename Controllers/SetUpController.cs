using Microsoft.AspNetCore.Mvc;

namespace UserManagementApi.Controllers
{
    public class SetUpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
