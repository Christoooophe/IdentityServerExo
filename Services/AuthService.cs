using Microsoft.AspNetCore.Identity;
using WebApplication3.Models;

namespace WebApplication3.Services
{
    public class AuthService: IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenGenerationService _tokenService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthService(UserManager<IdentityUser> userManager, ITokenGenerationService tokenGenerationService, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenGenerationService;
            _signInManager = signInManager;
        }


        public async Task<IdentityResult> RegisterUserAsync(RegisterRequest request)
        {
            var user = new IdentityUser { UserName = request.Email, Email = request.Email };

            return await _userManager.CreateAsync(user, request.Password);
        }


        public async Task<TokenResponse> AuthenticateUserAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new TokenResponse { AccessToken = null, TokenType = null, ExpiresIn = 0, Error = "Invalid credentials" };
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                return new TokenResponse { AccessToken = null, TokenType = null, ExpiresIn = 0, Error = "Account locked due to multiple failed login attempts. Try again later." };
            }

            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, true);
            if (!result.Succeeded)
            {
                await _userManager.AccessFailedAsync(user);
                if (await _userManager.GetAccessFailedCountAsync(user) >= 5)
                {
                    await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(1));
                    return new TokenResponse { AccessToken = null, TokenType = null, ExpiresIn = 0, Error = "Account locked due to multiple failed login attempts. Try again later." };
                }
                return new TokenResponse { AccessToken = null, TokenType = null, ExpiresIn = 0, Error = "Invalid credentials" };
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            return await _tokenService.GenerateTokenAsync(user, request.Password);
        }
    }
}
