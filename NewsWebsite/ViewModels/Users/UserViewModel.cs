using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.Users
{
    public class UserViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "The {0} field must be minimum {2} and maximum {1} symbols")]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$", ErrorMessage ="The {0} field must contain at least one letter and one number")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "The {0} field must be minimum {2} and maximum {1} symbols")]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-zA-Z])([a-zA-Z0-9]+)$", ErrorMessage = "The {0} field must contain at least one letter and one number")]
        public string ConfirmPassword { get; set; }
    }
}
