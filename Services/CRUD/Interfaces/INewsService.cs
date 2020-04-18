using DataAccess.Entities;
using Services.Auth.Interfaces;

namespace Services.CRUD.Interfaces
{
    public interface INewsService : IBaseCRUDService<News>, IItemLookup
    {
    }
}
