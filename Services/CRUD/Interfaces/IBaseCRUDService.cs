using DataAccess.Entities;
using DataAccess.Entities.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services.CRUD.Interfaces
{
    public interface IBaseCRUDService<TEntity> where TEntity : IBaseEntity
    {
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null);
        public Task<int> SaveAsync(TEntity entity);
    }
}
