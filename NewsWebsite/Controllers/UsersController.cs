using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Entities;
using Services.CRUD.Interfaces;
using NewsWebsite.ViewModels.Users;
using Services.CRUD.DTOs;
using NewsWebsite.Utils;
using Services.SMTP.Interfaces;
using Services.Transactions.Interfaces;
using NewsWebsite.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System.Linq;
using Microsoft.AspNetCore.Routing;

namespace NewsWebsite.Controllers
{
    public class UsersController : BaseViewsController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IUsersService usersService;
        private readonly ISMTPService smtpService;

        public UsersController(IUnitOfWork unitOfWork, IUsersService usersService, ISMTPService smtpService)
        {
            this.unitOfWork = unitOfWork;
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
        public async Task<IActionResult> Register(RegisterUserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }

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

            string emailConfirmationToken = await usersService.GenerateEmailConfirmationTokenAsync(userDb);
            string emailConfirmationLink = this.Url.Action(nameof(ConfirmEmail), this.GetControllerName(nameof(UsersController)), new { userDb.Email, emailConfirmationToken }, Request.Scheme);
            string emailConfirmation = string.Format("Hi {0}. You have just registered to NewsWebsite. To confirm your email address, please go to: {1}", userDb.UserName, emailConfirmationLink);
            this.smtpService.SendEmail("NewsWebsite Email Confirmation", emailConfirmation, userViewModel.Email);

            UsersServiceResultDTO addToRoleResultDTO = await this.usersService.AddToRoleAsync(userDb, RoleConstants.NormalUser);
            if (!addToRoleResultDTO.IsSucceed)
            {
                base.AddValidationErrorsToModelState(addToRoleResultDTO.ErrorMessages);
                return View(userViewModel);
            }

            TempData["SuccessMessage"] = "User registered successfully! In order to log in, please check the confirmation email that you have just received.";
            return base.RedirectToIndexActionInHomeController();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string email, string emailConfirmationToken)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(emailConfirmationToken))
            {
                return base.RedirectToIndexActionInHomeController();
            }

            User userDb = await this.usersService.FindByEmailAsync(email);

            if (userDb == null)
            {
                TempData["ErrorMessage"] = "A user with this email doesn't exist!";
                return View("ValidationErrorsWithoutSpecificModel");
            }

            UsersServiceResultDTO confirmEmailDTO = await this.usersService.ConfirmEmailAsync(userDb, emailConfirmationToken);

            if (!confirmEmailDTO.IsSucceed)
            {
                base.AddValidationErrorsToModelState(confirmEmailDTO.ErrorMessages);

                return View("ValidationErrorsWithoutSpecificModel");
            }

            TempData["SuccessMessage"] = "Email confirmed successfully!";
            return base.RedirectToIndexActionInHomeController();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserViewModel loginUserViewModel, string returnUrl)
        {
            SignInResultDTO signInResultDTO = await this.usersService.PasswordSignInAsync(loginUserViewModel.Username, loginUserViewModel.Password);

            if (signInResultDTO.IsNotAllowed)
            {
                User userDb = await this.usersService.FindByUsername(loginUserViewModel.Username);

                if (!userDb.EmailConfirmed && await this.usersService.CheckPasswordAsync(userDb, loginUserViewModel.Password)) // we also check if the password is correct, to prevent account enumeration
                {
                    TempData["ErrorMessage"] = "Not confirmed email! Please confirm it";
                    return View(loginUserViewModel);
                }
            }

            if (!signInResultDTO.IsSucceed)
            {
                TempData["ErrorMessage"] = "Wrong username or password";
                return View(loginUserViewModel);
            }

            TempData["SuccessMessage"] = string.Format("Welcome {0}", loginUserViewModel.Username);

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return LocalRedirect(returnUrl); // use LocalRedirect to prevent open redirect attack
            }

            return base.RedirectToIndexActionInHomeController();
        }

        [HttpPost]
        [Authorize]
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
            if (userDb != null && userDb.EmailConfirmed)
            {
                string passwordResetToken = await this.usersService.GeneratePasswordResetTokenAsync(userDb);

                string passwordResetLink = Url.Action(nameof(SetPassword), this.GetControllerName(nameof(UsersController)), new { userId = userDb.Id, passwordResetToken }, Request.Scheme);
                this.smtpService.SendEmail("NewsWebsite Reset password", string.Format("Hi {0}. You have requested to reset your pasword. To reset it please click here: {1} . If you haven't requested a password reset, please contact your system administrator because you may be at risk.", userDb.UserName, passwordResetLink), forgotPasswordViewModel.Email); // We intentionally use a space after the url, otherwise email providers will treat the dot as part of the url
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
            UsersServiceResultDTO resetPasswordResultDTO = await usersService.ResetPasswordAsync(userDb, setPasswordViewModel.PasswordResetToken, setPasswordViewModel.Password);
            if (!resetPasswordResultDTO.IsSucceed)
            {
                base.AddValidationErrorsToModelState(resetPasswordResultDTO.ErrorMessages);
                return View(setPasswordViewModel);
            }

            TempData["SuccessMessage"] = "Password set successfully!";
            return RedirectToIndexActionInHomeController();
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        [HttpGet]
        public async Task<IActionResult> Index(string filter, int pageindex = 1, string sortExpression = nameof(GetUserViewModel.Fullname))
        {
            List<GetUserViewModel> usersViewModels = new List<GetUserViewModel>();

            IQueryable<User> usersDb = this.usersService.GetAll();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                usersDb = usersDb.Where(x => x.UserName.Contains(filter));
            }

            await usersDb.ForEachAsync(userDb => usersViewModels.Add(
                new GetUserViewModel
                {
                    Id = userDb.Id,
                    Fullname = userDb.UserName,
                    Email = userDb.Email
                }
             )
            );

            PagingList<GetUserViewModel> pagingListViewModels = PagingList.Create(usersViewModels, 3, pageindex, sortExpression, nameof(GetUserViewModel.Fullname));
            pagingListViewModels.RouteValue = new RouteValueDictionary {
                { "filter", filter }
            };

            return View(pagingListViewModels);
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        [HttpGet]
        public async Task<IActionResult> ChangePasswordAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                ModelState.AddModelError(string.Empty, "Invalid user id");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToIndexActionInHomeController();
            }

            User userDb = await usersService.GetAll(x => x.Id == userId)
                                            .FirstOrDefaultAsync();

            if (userDb == null)
            {
                TempData["ErrorMessage"] = "A user with this id doesn't exist!";
                return RedirectToIndexActionInHomeController();
            }

            ChangePasswordViewModel changePasswordViewModel = new ChangePasswordViewModel
            {
                UserId = userDb.Id,
                Username = userDb.UserName
            };

            return View(changePasswordViewModel);
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        [HttpPost]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel changePasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(changePasswordViewModel);
            }

            User userDb = await usersService.GetAll(x => x.Id == changePasswordViewModel.UserId)
                                            .FirstOrDefaultAsync();

            if (userDb == null)
            {
                TempData["ErrorMessage"] = "A user with this id doesn't exist!";
                return RedirectToIndexActionInHomeController();
            }

            string passwordResetToken = await usersService.GeneratePasswordResetTokenAsync(userDb);
            UsersServiceResultDTO changePasswordResultDTO = await usersService.ResetPasswordAsync(userDb, passwordResetToken, changePasswordViewModel.Password);

            if (!changePasswordResultDTO.IsSucceed)
            {
                base.AddValidationErrorsToModelState(changePasswordResultDTO.ErrorMessages);
                return View(changePasswordViewModel);
            }

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToListAllActionInCurrentController();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return RedirectToIndexActionInHomeController(); // for security reasons, we do not show an Access Denied page
        }
    }
}
