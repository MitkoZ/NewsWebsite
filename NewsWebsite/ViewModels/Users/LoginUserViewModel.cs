using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.Users
{
    public class LoginUserViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
