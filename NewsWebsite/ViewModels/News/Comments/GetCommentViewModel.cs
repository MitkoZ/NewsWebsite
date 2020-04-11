using System.Collections.Generic;

namespace NewsWebsite.ViewModels.News.Comments
{
    public class GetCommentViewModel : PutCommentViewModel //TODO: better hierarchy?
    {
        public bool CreatedByCurrentUser { get; set; }
        public string Creator { get; set; }//TODO: should be creatorId
        public Dictionary<string, string> Pings { get; set; }
        public int UpvoteCount { get; set; }
        public bool UserHasUpvoted { get; set; }//TODO: should be IsUserHasUpvoted
    }
}
