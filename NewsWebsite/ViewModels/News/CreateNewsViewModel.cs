using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.News
{
    public class CreateNewsViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [HiddenInput]
        public string Content { get; set; }
    }
}
