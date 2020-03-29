using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace NewsWebsite.Utils
{
    public static class ControllersExtensionMethods
    {
        /// <summary>
        /// A way to get the controller name without the "Controller" suffix
        /// </summary>
        /// <param name="controllerBase">The controller</param>
        /// <param name="controllerName">The controller name</param>
        /// <returns>The controller name without a "Controller" suffix. Don't include the "Controller" keyword or the method will not work.</returns>
        public static string GetControllerName(this ControllerBase controllerBase, string controllerName)
        {
            return controllerName.Replace("Controller", string.Empty);
        }

        public static string GetCurrentUserId(this ControllerBase controllerBase)
        {
            return controllerBase.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
