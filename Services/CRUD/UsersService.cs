using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Repositories.Interfaces;
using Services.CRUD.DTOs;
using Services.CRUD.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Services.CRUD
{
    public class UsersService : BaseCRUDService<User>, IUsersService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public UsersService(IUsersRepository repository, UserManager<User> userManager, SignInManager<User> signInManager) : base(repository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<UsersServiceResultDTO> CreateAsync(User userDb, string password)
        {
            IdentityResult identityResult = await userManager.CreateAsync(userDb, password);
            UsersServiceResultDTO registerResult = new UsersServiceResultDTO();

            if (identityResult.Succeeded)
            {
                registerResult.IsSucceed = true;

                return registerResult;
            }

            registerResult.ErrorMessages = this.GetErrorMessages(identityResult);

            return registerResult;
        }
        public async Task<UsersServiceResultDTO> CreateAsync(User userDb)
        {
            IdentityResult identityResult = await userManager.CreateAsync(userDb);
            return GetUsersServiceResultDTO(identityResult);
        }

        private UsersServiceResultDTO GetUsersServiceResultDTO(IdentityResult identityResult)
        {
            UsersServiceResultDTO registerResult = new UsersServiceResultDTO();

            if (identityResult.Succeeded)
            {
                registerResult.IsSucceed = true;

                return registerResult;
            }

            registerResult.ErrorMessages = this.GetErrorMessages(identityResult);

            return registerResult;
        }

        private List<string> GetErrorMessages(IdentityResult identityResult)
        {
            List<string> errorMessages = new List<string>();

            identityResult.Errors.ToList().ForEach(identityError => errorMessages.Add(identityError.Description));

            return errorMessages;
        }

        public async Task<SignInResultDTO> PasswordSignInAsync(string username, string password)
        {
            SignInResult signInResult = await this.signInManager.PasswordSignInAsync(username, password, true, false);

            SignInResultDTO signInResultDTO = new SignInResultDTO
            {
                IsSucceed = signInResult.Succeeded
            };

            return signInResultDTO;
        }

        public bool IsSignedIn(IPrincipal principal)
        {
            bool isSignedIn = signInManager.IsSignedIn((ClaimsPrincipal)principal);

            return isSignedIn;
        }

        public async Task SignOutAsync()
        {
            await this.signInManager.SignOutAsync();
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User userDb)
        {
            return await this.userManager.GeneratePasswordResetTokenAsync(userDb);
        }

        public async Task<User> FindByIdAsync(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }

        public async Task<UsersServiceResultDTO> ResetPasswordAsync(User userDb, string passwordResetToken, string newPassword)
        {
            IdentityResult identityResult = await userManager.ResetPasswordAsync(userDb, passwordResetToken, newPassword);

            return this.GetUsersServiceResultDTO(identityResult);
        }

        public async Task<UsersServiceResultDTO> AddToRoleAsync(User userDb, string role)
        {
            IdentityResult identityResult = await this.userManager.AddToRoleAsync(userDb, role);

            return this.GetUsersServiceResultDTO(identityResult);
        }
    }
}
