using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.Reporters
{
    public class CreateReporterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
