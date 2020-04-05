﻿using DataAccess.Entities.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Services.CRUD.Interfaces
{
    public interface IBaseCRUDService<TEntity> where TEntity : IBaseEntity
    {
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null, bool isQueryDeletedRecords = false);
        void Save(TEntity entity);
        Task DeleteAsync(string id);
    }
}
