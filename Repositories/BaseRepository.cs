﻿using DataAccess;
using DataAccess.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class, IBaseEntity
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

        public virtual void Add(TEntity entity)
        {
            this.dbContext.Set<TEntity>().Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            this.dbContext.Set<TEntity>().Update(entity);
        }

        public async Task<int> SaveAsync(TEntity entity)
        {
            if (entity.Id == null)
            {
                this.Add(entity);
            }
            else
            {
                this.Update(entity);
            }

            return await dbContext.SaveChangesAsync();
        }
    }
}
