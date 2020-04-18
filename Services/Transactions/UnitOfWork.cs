using DataAccess;
using Services.Transactions.Interfaces;
using System.Threading.Tasks;

namespace Services.Transactions
{
    /// <summary>
    /// Since we use dependency injection, the same dbContext will be injected in the UnitOfWork and we do not need to manually create the repositories
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NewsDbContext dbContext;

        public UnitOfWork(NewsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> CommitAsync()
        {
            if (await dbContext.SaveChangesAsync() > 0)
            {
                return true;
            }

            return false;
        }
    }
}
