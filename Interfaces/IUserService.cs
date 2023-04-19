using UserManagementApi.Model.SignUp;
using UserManagementApi.Model;
using UserManagementApi.Model.Authentication.Login;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UserManagementApi.Interfaces
{
    public interface IUserService
    {
        public  Task<UserManagerResponse> Register(RegisterUser registerUser, string role);

        public Task<UserManagerResponse> ConfirmEmail(string token, string email);

        public Task<UserManagerResponse> Login(LoginModel loginModel);

        public Task<UserManagerResponse> LoginWithOTP(string code, string username);





    }
}
