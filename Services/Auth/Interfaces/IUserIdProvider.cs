using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Services.Auth.Interfaces
{
    public interface IUserProvider
    {
        string GetCurrentUserId(HttpContext context);
        Task<bool> IsInRoleAsync(string userId, string role);
    }
}
