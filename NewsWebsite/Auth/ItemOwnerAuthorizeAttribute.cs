using Microsoft.AspNetCore.Mvc.Filters;
using NewsWebsite.Auth.Interfaces;
using System;
using System.Threading.Tasks;
using Services.Auth.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using NewsWebsite.Utils;

namespace NewsWebsite.Auth
{
    /// <summary>
    /// Declarative resource authorization idea is taken from https://undocumented.dev/declarative-resource-based-authorisation-with-asp-net-core/
    /// </summary>
    public class ItemOwnerAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRule
    {
        private readonly string idArgumentName;
        private readonly Type itemLookupServiceType;

        /// <summary>
        /// Initliazes the ItemOwnerAuthorizeAttribute
        /// </summary>
        /// <param name="itemLookupServiceType">The type of the service used for getting the id value for the current resource from the database</param>
        /// <param name="idArgumentName">The id argument name by which to search in the HTTP Request. It's case sensitive and searches recursively if it's contained in a complex type</param>
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
            string resourceId = GetPropertyValue(idArgumentName, (Dictionary<string, object>)context.ActionArguments);
            if (resourceId != null)
            {
                string itemOwnerId = await itemLookup.GetOwnerId(resourceId);
                return currentUserId == itemOwnerId;
            }

            return false;
        }

        private string GetPropertyValue(string propertyName, Dictionary<string, object> argumentsDictionary)
        {
            foreach (string key in argumentsDictionary.Keys)
            {
                if (argumentsDictionary.TryGetValue(propertyName, out object value) && value is string)
                {
                    return value.ToString(); // the value was found
                }

                object complexTypeValue = argumentsDictionary[key]; // the null check is just to be sure the value doesn't come null from the front-end
                if (complexTypeValue != null && complexTypeValue.GetType().IsClass && IsUserDefined(complexTypeValue.GetType())) // it's a complex type, try searching inside it
                {
                    return GetPropertyValue(propertyName, complexTypeValue.ToDictionary());
                }
            }

            return null;
        }

        private bool IsUserDefined(Type type)
        {
            if (type.Assembly.FullName == Assembly.GetExecutingAssembly().FullName)
            {
                return true;
            }

            return false;
        }

    }
}
