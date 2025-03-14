namespace WebApplication3.Services
{
    public interface IRoleService
    {
        Task AssignRoleAsync(string email, string role);
    }
}
