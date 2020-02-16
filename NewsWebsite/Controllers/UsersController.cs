using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataAccess.Entities;
using Services.CRUD.Interfaces;
using NewsWebsite.ViewModels.Users;
using Services.CRUD.DTOs;
using NewsWebsite.Utils;
using Microsoft.Extensions.Logging;

namespace NewsWebsite.Controllers
{
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;

        public UsersController(ILogger<BaseController> logger, IUsersService usersService) : base(logger)
        {
            this.usersService = usersService;
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

                    RegisterResultDTO registerResultDTO = await usersService.CreateAsync(userDb, userViewModel.Password);

                    if (!registerResultDTO.IsSucceed)
                    {
                        foreach (string errorMessage in registerResultDTO.ErrorMessages)
                        {
                            ModelState.AddModelError(string.Empty, errorMessage);
                        }
                        return View(userViewModel);
                    }

                    SignInResultDTO signInResultDTO = await usersService.PasswordSignInAsync(userViewModel.Username, userViewModel.Password);

                    if (signInResultDTO.IsSucceed)
                    {
                        TempData["SuccessMessage"] = "User registered successfully!";
                        string homeControllerName = this.GetControllerName(nameof(HomeController));
                        return RedirectToAction(nameof(Index), homeControllerName);
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

                string homeControllerName = this.GetControllerName(nameof(HomeController));

                return RedirectToAction(nameof(Index), homeControllerName);
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
            return RedirectToAction(nameof(Index), this.GetControllerName(nameof(HomeController)));
        }
    }
}
