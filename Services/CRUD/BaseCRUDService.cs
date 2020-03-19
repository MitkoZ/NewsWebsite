﻿using DataAccess.Entities.Interfaces;
using Repositories.Interfaces;
using Services.CRUD.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services.CRUD
{
    public abstract class BaseCRUDService<TEntity> : IBaseCRUDService<TEntity>
        where TEntity : IBaseEntity
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

        public async Task<bool> SaveAsync(TEntity entity)
        {
            int savedEntities = await this.repository.SaveAsync(entity);

            if (savedEntities > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await this.repository.DeleteAsync(id) > 0;
        }
    }
}
