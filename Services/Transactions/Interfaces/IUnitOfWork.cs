using System.Threading.Tasks;

namespace Services.Transactions.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();
    }
}
