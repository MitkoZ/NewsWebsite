using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsWebsite.Utils;
using NewsWebsite.ViewModels.Reporters;
using Services.CRUD.DTOs;
using Services.CRUD.Interfaces;
using Services.SMTP.Interfaces;

namespace NewsWebsite.Controllers
{
    public class ReportersController : BaseController
    {
        private readonly ISMTPService smtpService;
        private readonly IUsersService usersService;

        public ReportersController(ILogger<ReportersController> logger, IUsersService usersService, ISMTPService smtpService) : base(logger)
        {
            this.smtpService = smtpService;
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

            UsersServiceResultDTO registerResultDTO = await this.usersService.CreateAsync(userDb);

            if (!registerResultDTO.IsSucceed)
            {
                base.AddValidationErrorsToModelState(registerResultDTO.ErrorMessages);
                return View(createReporterViewModel);
            }

            string passwordResetToken = await usersService.GeneratePasswordResetTokenAsync(userDb);

            string setPasswordAbsoluteUrl = Url.Action(nameof(UsersController.SetPassword), this.GetControllerName(nameof(UsersController)), new { userId = userDb.Id, passwordResetToken }, Request.Scheme);

            string setPasswordTextEmail = string.Format("Hi {0}, You have just been registered to our website by a system administrator. To complete your registration, please set your password by going to: {1}", userDb.UserName, setPasswordAbsoluteUrl);
            this.smtpService.SendEmail("NewsWebsite Complete Registration", setPasswordTextEmail, createReporterViewModel.Email);

            UsersServiceResultDTO addToRoleResultDTO = await this.usersService.AddToRoleAsync(userDb, "Reporter");
            if (!addToRoleResultDTO.IsSucceed)
            {
                base.AddValidationErrorsToModelState(addToRoleResultDTO.ErrorMessages);
                return View(createReporterViewModel);
            }

            TempData["SuccessMessage"] = "The reporter was created and has received an email to set his password";
            return RedirectToIndexActionInHomeController();
        }
    }
}