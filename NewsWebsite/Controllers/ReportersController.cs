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
            string setPasswordAbsoluteUrl = this.GenerateAbsoluteUrl(nameof(SetPassword), new { userId = userDb.Id, passwordResetToken });

            string setPasswordTextEmail = string.Concat("Hi ", userDb.UserName, ", ", "You have just been registered to our website by an system administrator. To complete your registration, please set your password by going to: ", setPasswordAbsoluteUrl);
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

        [HttpGet]
        public IActionResult SetPassword(string userId, string passwordResetToken)
        {
            if (string.IsNullOrWhiteSpace(passwordResetToken) || string.IsNullOrWhiteSpace(userId))
            {
                ModelState.AddModelError("", "Invalid input for setting the password"); // deliberately a little bit more cryptic validation message, in case of hackers 
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPasswordAsync(ReporterPasswordViewModel reporterPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(reporterPasswordViewModel);
            }

            User userDb = await usersService.FindByIdAsync(reporterPasswordViewModel.UserId);
            UsersServiceResultDTO registerResultDTO = await usersService.ResetPasswordAsync(userDb, reporterPasswordViewModel.PasswordResetToken, reporterPasswordViewModel.Password);
            if (!registerResultDTO.IsSucceed)
            {
                base.AddValidationErrorsToModelState(registerResultDTO.ErrorMessages);
                return View(reporterPasswordViewModel);
            }

            TempData["SuccessMessage"] = "Reporter registered successfully!";
            return RedirectToIndexActionInHomeController();
        }
    }
}