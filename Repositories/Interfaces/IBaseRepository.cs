using DataAccess.Entities.Abstractions.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Repositories.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : IBaseNormalEntity
    {
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null);
        void Save(TEntity entity);
        void Delete(string id);
    }
}
