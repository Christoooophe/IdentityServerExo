using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using WebApplication3.Models;

namespace WebApplication3.Services
{
    public class TokenGenerationService: ITokenGenerationService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TokenGenerationService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
        }

        public async Task<TokenResponse> GenerateTokenAsync(IdentityUser user, string password)
        {
            var tokenEndpoint = _configuration["IdentityServer:TokenEndpoint"];
            if (string.IsNullOrEmpty(tokenEndpoint))
            {
                throw new InvalidOperationException("TokenEndpoint is not configured properly in appsettings.json");
            }

            var tokenRequest = new Dictionary<string, string>
        {
            { "client_id", _configuration["IdentityServer:ClientId"] },
            { "client_secret", _configuration["IdentityServer:ClientSecret"] },
            { "grant_type", "password" },
            { "username", user.Email },
            { "password", password },
            { "scope", _configuration["IdentityServer:Scopes"] }
        };

            Console.WriteLine("Sending token request with:");
            Console.WriteLine($"Client ID: {_configuration["IdentityServer:ClientId"]}");
            Console.WriteLine($"Client Secret: {_configuration["IdentityServer:ClientSecret"]}");
            Console.WriteLine($"Token Endpoint: {tokenEndpoint}");


            var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(tokenRequest));
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Raw token response: {responseContent}");


            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Failed to retrieve token: {responseContent}");
            }

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true
            });

            return tokenResponse;
        }
    }
}
