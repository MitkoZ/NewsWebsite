namespace NewsWebsite.ViewModels.Users
{
    public class GetUserViewModel
    {
        public string Id { get; set; }
        public string Fullname { get; set; } // don't rename it, since we can't specify mappings in the jquery-comments front-end library (searchUsers ajax request) for it
        public string Email { get; set; } // don't rename it, since we can't specify mappings in the jquery-comments front-end library (searchUsers ajax request) for it
    }
}
