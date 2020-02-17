using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsWebsite.Utils;
using NewsWebsite.ViewModels.Reporters;
using Services.CRUD.DTOs;
using Services.CRUD.Interfaces;

namespace NewsWebsite.Controllers
{
    public class ReportersController : Controller
    {
        private readonly UserManager<User> userManager; //TODO: remove after it's abstracted away using a service
        private readonly IUsersService usersService;

        public ReportersController(IUsersService usersService, UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.usersService = usersService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateReporterViewModel createReporterViewModel)
        {
            User userDb = new User
            {
                UserName = createReporterViewModel.Username,
                Email = createReporterViewModel.Email
            };

            RegisterResultDTO registerResultDTO = await this.usersService.CreateAsync(userDb);


            if (!registerResultDTO.IsSucceed)
            {
                foreach (string errorMessage in registerResultDTO.ErrorMessages)
                {
                    ModelState.AddModelError(string.Empty, errorMessage);
                }
                return View(createReporterViewModel);
            }

            //TODO: generate a token for setting his account and send him an email to set his password using SMTP
            //TODO: add him to the Reporter role (which yet hasn't been created)
            TempData["SuccessMessage"] = "The reporter was created and has received an email to set his password";
            string homeControllerName = this.GetControllerName(nameof(HomeController));
            return RedirectToAction(nameof(Index), homeControllerName);
        }


        [HttpGet]
        public void SetPassword(Guid reporterTokenId)
        {
            // TODO: if a user with this tokenId exists, send him a form to change his password
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public void SetPassword(Guid reporterTokenId, ReporterPasswordViewModel reporterPasswordViewModel)
        {
            //TODO: get the user by the tokenId
            //TODO: change his password with the newly added one
            //userManager.AddPasswordAsync(,reporterPasswordViewModel.Password);
        }
    }
}