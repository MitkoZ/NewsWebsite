using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.Users
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
