using DataAccess;
using DataAccess.Entities;
using Repositories.Interfaces;
using System;

namespace Repositories
{
    public class CommentsRepository : BaseRepository<Comment>, ICommentsRepository
    {
        public CommentsRepository(NewsDbContext dbContext) : base(dbContext)
        {
        }

        public override void Add(Comment entity)
        {
            DateTime dateTimeUtcNow = DateTime.UtcNow;
            entity.CreatedAt = dateTimeUtcNow;
            entity.UpdatedAt = dateTimeUtcNow;

            base.Add(entity);
        }

        public override void Update(Comment entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            base.Update(entity);
        }
    }
}
