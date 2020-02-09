using DataAccess.Entities;
using Repositories.Interfaces;
using Services.CRUD.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services.CRUD
{
    public abstract class BaseCRUDService<TEntity> : IBaseCRUDService<TEntity>
        where TEntity : BaseEntity
    {
        private readonly IBaseRepository<TEntity> repository;

        public BaseCRUDService(IBaseRepository<TEntity> repository)
        {
            this.repository = repository;
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            return this.repository.GetAll(filter);
        }

        public async Task<int> SaveAsync(TEntity entity)
        {
            return await this.repository.SaveAsync(entity);
        }
    }
}
