using DataAccess.Entities;
using Repositories.Interfaces;
using Services.CRUD.Interfaces;

namespace Services.CRUD
{
    public class CommentsService : BaseCRUDService<Comment>, ICommentsService
    {
        public CommentsService(ICommentsRepository repository) : base(repository)
        {
        }
    }
}
