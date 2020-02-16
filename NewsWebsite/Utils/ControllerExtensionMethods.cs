using Microsoft.AspNetCore.Mvc;

namespace NewsWebsite.Utils
{
    public static class ControllerExtensionMethods
    {
        /// <summary>
        /// A way to get the controller name without the "Controller" suffix
        /// </summary>
        /// <param name="controller">The controller</param>
        /// <param name="controllerName">The controller name</param>
        /// <returns>The controller name without a "Controller" suffix. Don't include the "Controller" keyword or the method will not work.</returns>
        public static string GetControllerName(this Controller controller, string controllerName)
        {
            return controllerName.Replace("Controller", string.Empty);
        }
    }
}
