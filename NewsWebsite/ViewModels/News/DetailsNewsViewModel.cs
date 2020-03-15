using System;
using System.ComponentModel.DataAnnotations;

namespace NewsWebsite.ViewModels.News
{
    public class DetailsNewsViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ReporterId { get; set; }

        [Display(Name = "Reporter Name")]
        public string ReporterName { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Updated At")]
        public DateTime UpdatedAt { get; set; }
    }
}
