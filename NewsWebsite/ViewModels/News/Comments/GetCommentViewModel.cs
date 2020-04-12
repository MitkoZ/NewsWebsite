using System;
using System.Collections.Generic;

namespace NewsWebsite.ViewModels.News.Comments
{
    public class GetCommentViewModel : PutCommentViewModel
    {
        public bool IsCreatedByCurrentUser { get; set; }
        public string CreatorId { get; set; }
        public Dictionary<string, string> Pings { get; set; }
        public int UpvoteCount { get; set; }
        public bool IsUserUpvoted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
