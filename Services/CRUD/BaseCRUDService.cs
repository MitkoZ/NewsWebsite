using DataAccess.Entities.Abstractions.Interfaces;
using LinqKit;
using Repositories.Interfaces;
using Services.CRUD.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Services.CRUD
{
    public abstract class BaseCRUDService<TEntity> : IBaseCRUDService<TEntity>
        where TEntity : IBaseNormalEntity
    {
        protected readonly IBaseRepository<TEntity> repository;

        public BaseCRUDService(IBaseRepository<TEntity> repository)
        {
            this.repository = repository;
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null, bool isQueryDeletedRecords = false)
        {
            Expression<Func<TEntity, bool>> isQueryDeletedRecordsFilter = x => x.IsDeleted == isQueryDeletedRecords;
            if (filter != null)
            {
                isQueryDeletedRecordsFilter = isQueryDeletedRecordsFilter.And(filter);
            }

            return this.repository.GetAll(isQueryDeletedRecordsFilter);
        }

        public void Save(TEntity entity)
        {
            this.repository.Save(entity);
        }

        public virtual void Delete(string id)
        {
            this.repository.Delete(id);
        }

    }
}
