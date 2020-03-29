using DataAccess.Entities.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : IBaseEntity
    {
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null);
        Task<int> SaveAsync(TEntity entity);
        Task<int> DeleteAsync(string id);
    }
}
