﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsWebsite.Utils;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace NewsWebsite.Controllers
{
    public abstract class BaseViewsController : Controller
    {
        protected readonly ILogger<BaseViewsController> logger;

        public BaseViewsController(ILogger<BaseViewsController> logger)
        {
            this.logger = logger;
        }

        protected IActionResult RedirectToIndexActionInHomeController()
        {
            string homeControllerName = this.GetControllerName(nameof(HomeController));
            return RedirectToAction(nameof(Index), homeControllerName);
        }

        protected IActionResult RedirectToListAllActionInCurrentController()
        {
            return RedirectToAction("ListAll"); //TODO: replace with strong typing (nameof) when you make a base listing action 
        }

        protected void AddValidationErrorsToModelState(List<string> errorMessages)
        {
            foreach (string errorMessage in errorMessages)
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
        }
    }
}