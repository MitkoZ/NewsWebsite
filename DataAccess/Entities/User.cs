using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities
{
    public class User : BaseEntity
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string HashedPassword { get; set; }

        [Required]
        public string Salt { get; set; }
    }
}
