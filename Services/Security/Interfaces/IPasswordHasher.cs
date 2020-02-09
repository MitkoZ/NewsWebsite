namespace Services.Security.Interfaces
{
    public interface IPasswordHasher
    {
        public ISaltAndSaltedHashDTO GenerateSaltAndSaltedHash(string plainTextPassword);
        public bool IsSamePassword(string plainTextPassword, string saltedHash, string salt);
    }
}
