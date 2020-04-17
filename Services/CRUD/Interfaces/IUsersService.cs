using DataAccess.Entities;
using Services.Auth.Interfaces;
using Services.CRUD.DTOs;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Services.CRUD.Interfaces
{
    public interface IUsersService : IBaseCRUDService<User>, IUserProvider
    {
        Task<UsersServiceResultDTO> CreateAsync(User userDb, string password);
        Task<UsersServiceResultDTO> CreateAsync(User userDb);
        Task<SignInResultDTO> PasswordSignInAsync(string username, string password);
        bool IsSignedIn(IPrincipal principal);
        Task SignOutAsync();
        Task<string> GeneratePasswordResetTokenAsync(User userDb);
        Task<User> FindByIdAsync(string userId);
        Task<UsersServiceResultDTO> ResetPasswordAsync(User userDb, string token, string newPassword);
        Task<UsersServiceResultDTO> AddToRoleAsync(User userDb, string role);
        Task<User> FindByEmailAsync(string email);
        Task<string> GenerateEmailConfirmationTokenAsync(User userDb);
        Task<UsersServiceResultDTO> ConfirmEmailAsync(User userDb, string token);
        Task<User> FindByUsername(string username);
        Task<bool> CheckPasswordAsync(User userDb, string password);
    }
}
