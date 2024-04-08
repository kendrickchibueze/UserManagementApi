using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using UserManagementApi.Interfaces;
using UserManagementApi.Model;
using UserManagementApi.Model.Authentication.Login;
using UserManagementApi.Model.SignUp;
using UserManagementApi.Services.Models;
using UserManagementApi.Services.Services;

namespace UserManagementApi.Services
{
    public class UserServices : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IUrlHelper _urlHelper;
        private readonly IConfiguration _configuration;
        public UserServices(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration, IEmailService emailService, IUrlHelper urlHelper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
           _emailService = emailService;
            _urlHelper = urlHelper;
            _configuration = configuration;
        }
        public async Task<UserManagerResponse> Register(RegisterUser registerUser, string role)
        {

            if (registerUser == null || string.IsNullOrEmpty(registerUser.Email))
            {
                throw new ArgumentException("RegisterUser or its Email property is null or empty.");
            }
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                throw new NullReferenceException(ErrorMsg.NullModel);
            }

            //Add the User in the database
            IdentityUser user = new()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username,
                TwoFactorEnabled = true
            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.Password);
                if (!result.Succeeded)
                {
                    return new UserManagerResponse
                    {
                        Message = ErrorMsg.UserNotCreated,
                        IsSuccess = false,
                        Errors = result.Errors.Select(e => e.Description)
                    };
                }
                //Add role to the user....

                await _userManager.AddToRoleAsync(user, role);

                //Add Token to Verify the email....
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = _urlHelper.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, _urlHelper.ActionContext.HttpContext.Request.Scheme);
                var message = new Message(new string[] { user.Email! }, "Confirmation email link", confirmationLink!);
                _emailService.SendEmail(message);


                return new UserManagerResponse
                {
                    Message = $"User created & Email Sent to {user.Email} SuccessFully",
                    IsSuccess = true,
                };
            }
            else
            {

                return new UserManagerResponse
                {
                    Message = ErrorMsg.UserRoleNotFound,
                    IsSuccess = false
                };
            }
        }
       public async Task<UserManagerResponse> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return new UserManagerResponse
                    {
                        Message = Msg.EmailConfirm,
                        IsSuccess = true,
                       
                    };
                }
            }

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = ErrorMsg.InvalidUser + " " + email,
                
            };
        }
        public async Task<UserManagerResponse> Login(LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user == null)
            {
                return new UserManagerResponse
                {
                    Message = ErrorMsg.InvalidUser,
                    IsSuccess = false,
                    Errors = new[] { ErrorMsg.InvalidLoginAttempt }
                };
            }

            if (user.TwoFactorEnabled)
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, true);
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                var message = new Message(new string[] { user.Email! }, "OTP Confrimation", token);
                _emailService.SendEmail(message);

                return new UserManagerResponse
                {
                    Message = $"We have sent an OTP to your Email {user.Email}",
                    IsSuccess = true,
                };
            }

            if (!await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                return new UserManagerResponse
                {
                    Message = ErrorMsg.InvalidPassword,
                    IsSuccess = false,
                    Errors = new[] { ErrorMsg.InvalidLoginAttempt }
                };
            }
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtToken = GetToken(authClaims);

            return new UserManagerResponse
            {
                IsSuccess = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiredDate = jwtToken.ValidTo
            };
        }
        public async Task<UserManagerResponse> LoginWithOTP(string code, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);

            if (signIn.Succeeded)
            {
                if (user != null)
                {
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };
                    var userRoles = await _userManager.GetRolesAsync(user);
                    foreach (var role in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var jwtToken = GetToken(authClaims);

                    return new UserManagerResponse
                    {
                        IsSuccess = true,
                        Message = $"User {username} successfully logged in.",
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        ExpiredDate = jwtToken.ValidTo
                    };
                }
            }

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Invalid code entered. Please try again."
            };
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
