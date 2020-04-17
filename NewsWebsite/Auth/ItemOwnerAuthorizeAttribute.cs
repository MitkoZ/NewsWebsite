using Microsoft.AspNetCore.Mvc.Filters;
using NewsWebsite.Auth.Interfaces;
using System;
using System.Threading.Tasks;
using Services.Auth.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace NewsWebsite.Auth
{
    /// <summary>
    /// Declarative resource authorization idea is taken from https://undocumented.dev/declarative-resource-based-authorisation-with-asp-net-core/
    /// </summary>
    public class ItemOwnerAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRule
    {
        private readonly string idArgumentName;
        private readonly Type itemLookupServiceType;

        public ItemOwnerAuthorizeAttribute(Type itemLookupServiceType, string idArgumentName = "id") // Attributes can't have DI because they need compile time parameters
        {
            this.idArgumentName = idArgumentName;
            this.itemLookupServiceType = typeof(IItemLookup).IsAssignableFrom(itemLookupServiceType) ? itemLookupServiceType : throw new ArgumentException($"Type: {itemLookupServiceType.Name} is not a valid {nameof(IItemLookup)} type."); // We use this hack, in order to allow immitate dependency injection in the attribute (by the ServiceLocator)
        }

        public async Task<bool> IsHaveAccess(ActionExecutingContext context)
        {
            IServiceProvider serviceProvider = context.HttpContext.RequestServices;

            IItemLookup itemLookup = serviceProvider.GetService(itemLookupServiceType) as IItemLookup;
            IUserProvider userIdProvider = serviceProvider.GetService<IUserProvider>();

            string currentUserId = userIdProvider.GetCurrentUserId(context.HttpContext);

            bool isCurrentUserAdmin = await userIdProvider.IsInRoleAsync(currentUserId, RoleConstants.Administrator);
            if (isCurrentUserAdmin) // admin has access to everything
            {
                return true;
            }

            //Find the relevant item id from the request
            if (context.ActionArguments.TryGetValue(idArgumentName, out object argument) && argument is string resourceId)
            {
                string itemOwnerId = await itemLookup.GetOwnerId(resourceId);
                return currentUserId == itemOwnerId;
            }

            return false;
        }
    }
}
