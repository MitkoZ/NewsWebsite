using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.News
{
    public class CreateNewsViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
