using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.CRUD.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.CRUD
{
    public class CommentsService : BaseCRUDService<Comment>, ICommentsService
    {
        public CommentsService(ICommentsRepository repository) : base(repository)
        {
        }

        public override async Task DeleteAsync(string id)
        {
            List<Comment> subCommentsDb = await base.repository.GetAll(x => x.ParentId == id)
                                                         .ToListAsync(); // if the collection is empty, the foreach won't execute

            subCommentsDb.ForEach(async subComment =>
            {
                await this.DeleteAsync(subComment.Id); // deletes subcomments recursively
            });

            await base.DeleteAsync(id); // deletes main comment
        }
    }
}
