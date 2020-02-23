using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Entities;
using Services.CRUD.Interfaces;
using NewsWebsite.ViewModels.Users;
using Services.CRUD.DTOs;
using NewsWebsite.Utils;
using Microsoft.Extensions.Logging;
using Services.SMTP.Interfaces;

namespace NewsWebsite.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly ISMTPService smtpService;

        public UsersController(ILogger<BaseController> logger, IUsersService usersService, ISMTPService smtpService) : base(logger)
        {
            this.usersService = usersService;
            this.smtpService = smtpService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel userViewModel)
        {
            try
            {
                //TODO: add remote validation for username and email
                if (ModelState.IsValid)
                {
                    User userDb = new User
                    {
                        UserName = userViewModel.Username,
                        Email = userViewModel.Email
                    };

                    UsersServiceResultDTO registerResultDTO = await usersService.CreateAsync(userDb, userViewModel.Password);

                    if (!registerResultDTO.IsSucceed)
                    {
                        base.AddValidationErrorsToModelState(registerResultDTO.ErrorMessages);
                        return View(userViewModel);
                    }

                    SignInResultDTO signInResultDTO = await usersService.PasswordSignInAsync(userViewModel.Username, userViewModel.Password);

                    if (signInResultDTO.IsSucceed)
                    {
                        TempData["SuccessMessage"] = "User registered successfully!";
                        return base.RedirectToIndexActionInHomeController();
                    }
                }

                return View(userViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ooops, something went wrong";
                logger.LogError(ex, "A exception with the following input occured: {@userViewModel}", userViewModel);
                return View(userViewModel);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserViewModel loginUserViewModel)
        {
            try
            {
                SignInResultDTO signInResultDTO = await this.usersService.PasswordSignInAsync(loginUserViewModel.Username, loginUserViewModel.Password);
                if (!signInResultDTO.IsSucceed)
                {
                    ViewBag.ErrorMessage = "Wrong username or password";
                    return View(loginUserViewModel);
                }

                TempData["SuccessMessage"] = string.Format("Welcome {0}", loginUserViewModel.Username);

                return base.RedirectToIndexActionInHomeController();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ooops, something went wrong";
                logger.LogError(ex, "A exception with the following input occured: {@loginUserViewModel}", loginUserViewModel);
                return View(loginUserViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogoutAsync()
        {
            await this.usersService.SignOutAsync();
            return base.RedirectToIndexActionInHomeController();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordViewModel forgotPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordViewModel);
            }

            User userDb = await this.usersService.FindByEmailAsync(forgotPasswordViewModel.Email);
            if (userDb != null)//TODO: check if email was previously confirmed
            {
                string passwordResetToken = await this.usersService.GeneratePasswordResetTokenAsync(userDb);

                string passwordResetLink = Url.Action(nameof(SetPassword), this.GetControllerName(nameof(UsersController)), new { userId = userDb.Id, passwordResetToken }, Request.Scheme);
                this.smtpService.SendEmail("NewsWebsite Reset password", string.Format("Hi {0}. You have requested to reset your pasword. To reset it please click here: {1}. If you haven't requested a password reset, please contact your system administrator because you may be at risk.", userDb.UserName, passwordResetLink), forgotPasswordViewModel.Email);
            }

            return View("ForgotPasswordConfirmation"); // We send the same response no matter if a user user with this email exists or not, so we can prevent account enumeration
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
        public async Task<IActionResult> SetPasswordAsync(SetPasswordViewModel setPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(setPasswordViewModel);
            }

            User userDb = await usersService.FindByIdAsync(setPasswordViewModel.UserId);
            UsersServiceResultDTO registerResultDTO = await usersService.ResetPasswordAsync(userDb, setPasswordViewModel.PasswordResetToken, setPasswordViewModel.Password);
            if (!registerResultDTO.IsSucceed)
            {
                base.AddValidationErrorsToModelState(registerResultDTO.ErrorMessages);
                return View(setPasswordViewModel);
            }

            TempData["SuccessMessage"] = "Password set successfully!";
            return RedirectToIndexActionInHomeController();
        }
    }
}
