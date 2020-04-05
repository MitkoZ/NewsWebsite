namespace NewsWebsite.ViewModels.Users
{
    public class GetUserViewModel
    {
        public string Fullname { get; set; } //don't rename it, since we can't specify mappings in the front-end for it
        public string Email { get; set; }
    }
}
