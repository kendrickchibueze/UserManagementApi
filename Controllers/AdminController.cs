using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace UserManagementApi.Controllers
{
    
        [Authorize(Roles = "Admin")]
        [Route("api/[controller]")]
        [ApiController]
        public class AdminController : ControllerBase
        {
            [HttpGet("employees")]
            public IEnumerable<string> Get()
            {
                return new List<string> { "Ahmed", "Ali", "Ahsan" };
            }
        
        }
}
