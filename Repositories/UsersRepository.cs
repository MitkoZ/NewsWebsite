using DataAccess;
using DataAccess.Entities;
using Repositories.Interfaces;

namespace Repositories
{
    public class UsersRepository : BaseRepository<User>, IUsersRepository
    {
        public UsersRepository(NewsDbContext dbContext) : base(dbContext)
        {
        }
    }
}
