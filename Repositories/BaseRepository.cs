using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : BaseEntity
    {
        private readonly DbContext dbContext;

        public BaseRepository(NewsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            DbSet<TEntity> dbSet = this.dbContext
                                       .Set<TEntity>();

            if (filter != null)
            {
                return dbSet
                       .Where(filter);
            }

            return dbSet;
        }

        public async Task<int> SaveAsync(TEntity entity)
        {
            this.dbContext.Set<TEntity>().Add(entity);

            return await dbContext.SaveChangesAsync();
        }
    }
}
