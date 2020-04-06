using System;

namespace NewsWebsite.ViewModels.News.Comments
{
    public class PostCommentViewModel
    {
        public string ParentId { get; set; }
        public DateTime CreatedAt { get; set; } //TODO: can we remove it? since we are setting it at the backend
        public DateTime UpdatedAt { get; set; } //TODO: can we remove it? since we are setting it at the backend
        public string Content { get; set; }
        public string Fullname { get; set; }
    }
}
