using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NewsWebsite.Auth.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsWebsite.Auth
{
    public class ResourceAuthorizationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IEnumerable<Task<bool>> filters = context
                                              .Filters
                                              .OfType<IAuthorizationRule>()
                                              .Select(filter => filter.IsHaveAccess(context));

            bool[] isHaveAccess = await Task.WhenAll(filters);

            if (!isHaveAccess.All(a => a))
            {
                //At least one filter.HasAccess returned false
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
