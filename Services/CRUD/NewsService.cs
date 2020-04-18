using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Services.CRUD.Interfaces;
using System.Threading.Tasks;

namespace Services.CRUD
{
    public class NewsService : BaseCRUDService<News>, INewsService
    {
        public NewsService(INewsRepository repository) : base(repository)
        {
        }

        public async Task<string> GetOwnerId(string itemId)
        {
            return (await this.repository
                          .GetAll(news => news.Id == itemId)
                          .FirstOrDefaultAsync())
                          .UserId;
        }
    }
}
