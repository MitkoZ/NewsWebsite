namespace Services.Security.Interfaces
{
    public interface ISaltAndSaltedHashDTO
    {
        /// <summary>
        /// The already generated Salt
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// The already generated Salted Hash
        /// </summary>
        public string SaltedHash { get; set; }
    }
}
