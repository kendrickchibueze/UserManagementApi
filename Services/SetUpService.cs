using Microsoft.AspNetCore.Identity;
using UserManagementApi.Model;

namespace UserManagementApi.Services
{
    public class SetUpService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<SetUpService> _logger;
        public SetUpService( ApplicationDbContext context, UserManager<IdentityUser> userManager,
        ILogger<SetUpService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            
        }
    }
}
