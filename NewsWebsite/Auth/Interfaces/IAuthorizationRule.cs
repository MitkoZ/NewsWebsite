using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace NewsWebsite.Auth.Interfaces
{
    public interface IAuthorizationRule : IFilterMetadata
    {
        Task<bool> IsHaveAccess(ActionExecutingContext context);
    }
}
