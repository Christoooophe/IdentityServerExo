using Microsoft.AspNetCore.Identity;
using WebApplication3.Models;

namespace WebApplication3.Services
{
    public interface ITokenGenerationService
    {
        Task<TokenResponse> GenerateTokenAsync(IdentityUser user, string password);
    }
}
