using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.Reporters
{
    public class RegisterReporterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
