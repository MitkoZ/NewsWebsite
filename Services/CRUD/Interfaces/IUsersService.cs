using DataAccess.Entities;
using Services.CRUD.DTOs;
using System.Threading.Tasks;

namespace Services.CRUD.Interfaces
{
    public interface IUsersService : IBaseCRUDService<User>
    {
        public Task<RegisterResultDTO> CreateAsync(User userDb, string password);
        public Task<SignInResultDTO> PasswordSignInAsync(string username, string password);
    }
}
