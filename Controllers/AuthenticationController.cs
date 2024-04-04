using Microsoft.AspNetCore.Mvc;
using UserManagementApi.Interfaces;
using UserManagementApi.Model;
using UserManagementApi.Model.Authentication.Login;
using UserManagementApi.Model.SignUp;

namespace UserManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserManagerResponse>> Register([FromBody] RegisterUser registerUser, string role)
        {
            var result = await _userService.Register(registerUser, role);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult<UserManagerResponse>> ConfirmEmail(string token, string email)
        {
            var result = await _userService.ConfirmEmail(token, email);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserManagerResponse>> Login([FromBody] LoginModel loginModel)
        {
            var result = await _userService.Login(loginModel);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("login-2FA")]
        public async Task<ActionResult<UserManagerResponse>> LoginWithOTP(string code, string username)
        {
            var result = await _userService.LoginWithOTP(code, username);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
