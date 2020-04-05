using System;

namespace NewsWebsite.ViewModels.News.Comments
{
    public class PostCommentViewModel
    {
        public string ParentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Content { get; set; }
        public string Fullname { get; set; }
    }
}
