using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using DataAccess.Entities;
using Services.CRUD.Interfaces;
using Services.Security.Interfaces;
using NewsWebsite.ViewModels.Users;

namespace NewsWebsite.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IPasswordHasher passwordHasher;

        public UsersController(NewsDbContext context, IUsersService usersService, IPasswordHasher passwordHasher)
        {
            this.usersService = usersService;
            this.passwordHasher = passwordHasher;
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
            //TODO: add remote validation for username and email
            if (ModelState.IsValid)
            {
                User userDb = new User();
                userDb.Username = userViewModel.Username;
                userDb.Email = userViewModel.Email;

                ISaltAndSaltedHashDTO saltAndSaltedHashDTO = this.passwordHasher.GenerateSaltAndSaltedHash(userViewModel.Password);
                userDb.Salt = saltAndSaltedHashDTO.Salt;
                userDb.HashedPassword = saltAndSaltedHashDTO.SaltedHash;

                await usersService.SaveAsync(userDb);

                TempData["SuccessMessage"] = "User registered successfully!";
                string controllerName = nameof(HomeController).Replace("Controller", ""); // we use the Replace method, so that we can use strong typing TODO: extract in a helper method
                return RedirectToAction(nameof(Index), controllerName);
            }
            return View(userViewModel);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserViewModel loginUserViewModel)
        {
            User userDb = await this.usersService.GetAll(x => x.Username == loginUserViewModel.Username).FirstOrDefaultAsync(); //TODO: verification of user before login

            if (userDb == null || !passwordHasher.IsSamePassword(loginUserViewModel.Password, userDb.HashedPassword, userDb.Salt))
            {
                ViewBag.ErrorMessage = "Wrong username or password";
                return View(loginUserViewModel);

            }

            TempData["SuccessMessage"] = string.Format("Welcome {0}", userDb.Username);
            string controllerName = nameof(HomeController).Replace("Controller", ""); // we use the Replace method, so that we can use strong typing
            return RedirectToAction(nameof(Index), controllerName);
        }
    }
}
