using Services.Security.Interfaces;
using System;
using System.Security.Cryptography;

namespace Services.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public class SaltAndSaltedHashDTO : ISaltAndSaltedHashDTO
        {
            public string Salt { get; set; }
            public string SaltedHash { get; set; }
        }

        private const int SALT_SIZE = 16;
        private const int HASH_SIZE = 20;
        private const int HASH_ITERATIONS = 10000;

        /// <summary>
        /// Use this method when you want to generate a Salt and a SaltedHash. After that get them from the properties.
        /// </summary>
        /// <param name="plainTextPassword">The password for which you want to generate a Salt and a SaltedHash</param>
        /// <returns>A Data Transfer Object that contains the newly created Salt and SaltedHash</returns>
        public ISaltAndSaltedHashDTO GenerateSaltAndSaltedHash(string plainTextPassword)
        {
            byte[] saltBytes = GenerateSaltBytes();
            byte[] saltedHashBytes = GenerateSaltedHashBytes(plainTextPassword, saltBytes);

            SaltAndSaltedHashDTO saltAndSaltedHashDTO = new SaltAndSaltedHashDTO();

            saltAndSaltedHashDTO.Salt = Convert.ToBase64String(saltBytes);
            saltAndSaltedHashDTO.SaltedHash = Convert.ToBase64String(saltedHashBytes);

            return saltAndSaltedHashDTO;
        }

        private byte[] GenerateSaltedHashBytes(string plainTextPassword, byte[] saltBytes)
        {
            return new Rfc2898DeriveBytes(plainTextPassword, saltBytes, PasswordHasher.HASH_ITERATIONS).GetBytes(PasswordHasher.HASH_SIZE);
        }

        private byte[] GenerateSaltBytes()
        {
            RNGCryptoServiceProvider randomNumberGenerator = new RNGCryptoServiceProvider();

            byte[] saltBytes = new byte[PasswordHasher.SALT_SIZE];
            randomNumberGenerator.GetBytes(saltBytes);

            return saltBytes;
        }

        /// <summary>
        /// Verifies that the password produces the same salted hash
        /// </summary>
        /// <param name="plainTextPassword">The password with which you will compare the salted hash</param>
        /// <param name="saltedHash">The previously generated salted hash. (probably stored in a database)</param>
        /// <param name="salt">The previously generated salt. (probably stored in a database)</param>
        /// <returns>True: If the password produces the same salted hash. Else: False</returns>
        public bool IsSamePassword(string plainTextPassword, string saltedHash, string salt) // We use Base64String for comparison, since it's easier to manipulate, rather than using array comparison
        {
            byte[] newSaltedHashBytes = GenerateSaltedHashBytes(plainTextPassword, Convert.FromBase64String(salt));
            string newSaltedHash = Convert.ToBase64String(newSaltedHashBytes);

            if (newSaltedHash == saltedHash)
            {
                return true;
            }

            return false;
        }
    }
}
