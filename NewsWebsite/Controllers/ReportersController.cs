using System.Threading.Tasks;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsWebsite.Utils;
using NewsWebsite.ViewModels.Reporters;
using Services.CRUD.DTOs;
using Services.CRUD.Interfaces;
using Services.SMTP.Interfaces;

namespace NewsWebsite.Controllers
{
    public class ReportersController : BaseViewsController
    {
        private readonly ISMTPService smtpService;
        private readonly IUsersService usersService;

        public ReportersController(ILogger<ReportersController> logger, IUsersService usersService, ISMTPService smtpService) : base(logger)
        {
            this.smtpService = smtpService;
            this.usersService = usersService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterReporterViewModel registerReporterViewModel)
        {
            User userDb = new User
            {
                UserName = registerReporterViewModel.Username,
                Email = registerReporterViewModel.Email
            };

            UsersServiceResultDTO registerResultDTO = await this.usersService.CreateAsync(userDb);

            if (!registerResultDTO.IsSucceed)
            {
                base.AddValidationErrorsToModelState(registerResultDTO.ErrorMessages);
                return View(registerReporterViewModel);
            }

            string passwordResetToken = await usersService.GeneratePasswordResetTokenAsync(userDb);

            string setPasswordAbsoluteUrl = Url.Action(nameof(UsersController.SetPassword), this.GetControllerName(nameof(UsersController)), new { userId = userDb.Id, passwordResetToken }, Request.Scheme);

            string setPasswordTextEmail = string.Format("Hi {0}, You have just been registered to our website by a system administrator. To complete your registration, please set your password by going to: {1}", userDb.UserName, setPasswordAbsoluteUrl);
            this.smtpService.SendEmail("NewsWebsite Complete Registration", setPasswordTextEmail, registerReporterViewModel.Email);

            UsersServiceResultDTO addToRoleResultDTO = await this.usersService.AddToRoleAsync(userDb, "Reporter");
            if (!addToRoleResultDTO.IsSucceed)
            {
                base.AddValidationErrorsToModelState(addToRoleResultDTO.ErrorMessages);
                return View(registerReporterViewModel);
            }

            UsersServiceResultDTO usersServiceResultDTO = await this.usersService.ConfirmEmailAsync(userDb, await this.usersService.GenerateEmailConfirmationTokenAsync(userDb)); // We need to confirm the email right after the registration of the reporter,
            //since we have enabled email confirmation (options.SignIn.RequireConfirmedEmail = true;) in the Startup file and the UsersController login won't let us in.
            if (!usersServiceResultDTO.IsSucceed)
            {
                base.AddValidationErrorsToModelState(usersServiceResultDTO.ErrorMessages);
                return View(registerReporterViewModel);
            }

            TempData["SuccessMessage"] = "The reporter was created and has received an email to set his password";
            return RedirectToIndexActionInHomeController();
        }
    }
}