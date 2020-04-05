using Microsoft.AspNetCore.Mvc;

namespace NewsWebsite.ViewModels.News.Comments
{
    public class PutCommentViewModel : PostCommentViewModel
    {
        [FromRoute]
        public string Id { get; set; }
    }
}
