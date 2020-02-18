using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.Reporters
{
    public class ReporterPasswordViewModel
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
        public string UserId { get; set; }

        [Required]
        public string passwordResetToken { get; set; }
    }
}
