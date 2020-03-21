using DataAccess.Entities;
using Repositories.Interfaces;
using Services.CRUD.Interfaces;

namespace Services.CRUD
{
    public class NewsService : BaseCRUDService<News>, INewsService
    {
        public NewsService(INewsRepository repository) : base(repository)
        {
        }
    }
}
