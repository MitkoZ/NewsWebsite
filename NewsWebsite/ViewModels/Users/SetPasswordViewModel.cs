using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.Users
{
    public class SetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        [HiddenInput]
        public string UserId { get; set; }

        [Required]
        [HiddenInput]
        public string PasswordResetToken { get; set; }
    }
}
