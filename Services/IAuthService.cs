using Microsoft.AspNetCore.Identity;
using WebApplication3.Models;

namespace WebApplication3.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterRequest request);
        Task<TokenResponse> AuthenticateUserAsync(LoginRequest login);

    }
}
