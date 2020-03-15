using DataAccess;
using DataAccess.Entities;
using Repositories.Interfaces;
using System;

namespace Repositories
{
    public class NewsRepository : BaseRepository<News>, INewsRepository
    {
        public NewsRepository(NewsDbContext dbContext) : base(dbContext)
        {
        }

        public override void Add(News entity)
        {
            DateTime dateTimeUtcNow = DateTime.UtcNow;
            entity.CreatedAt = dateTimeUtcNow;
            entity.UpdatedAt = dateTimeUtcNow;

            base.Add(entity);
        }

        public override void Update(News entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            base.Update(entity);
        }
    }
}
