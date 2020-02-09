using DataAccess.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null);
        public Task<int> SaveAsync(TEntity entity);
    }
}
