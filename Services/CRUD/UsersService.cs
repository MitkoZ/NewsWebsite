using DataAccess.Entities;
using Repositories.Interfaces;
using Services.CRUD.Interfaces;

namespace Services.CRUD
{
    public class UsersService : BaseCRUDService<User>, IUsersService
    {
        public UsersService(IUsersRepository repository) : base(repository)
        {
        }
    }
}
