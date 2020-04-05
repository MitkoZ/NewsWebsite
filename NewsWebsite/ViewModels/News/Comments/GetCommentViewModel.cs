namespace NewsWebsite.ViewModels.News.Comments
{
    public class GetCommentViewModel : PutCommentViewModel //TODO: better hierarchy?
    {
        public bool CreatedByCurrentUser { get; set; }
    }
}
