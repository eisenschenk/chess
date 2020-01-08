using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest
{
    public static class PasswordHelper
    {
        private const int PASSWORD_LENGTH = 20;
        private const int SALT_LENGTH = 16;

        public static string HashAndSalt(string password)
        {
            if (password is null)
                return null;

            byte[] salt = new byte[SALT_LENGTH];
            new RNGCryptoServiceProvider().GetBytes(salt);
            var result = new byte[PASSWORD_LENGTH + SALT_LENGTH];
            Array.Copy(HashString(password, salt), 0, result, 0, PASSWORD_LENGTH);
            Array.Copy(salt, 0, result, PASSWORD_LENGTH, SALT_LENGTH);
            return Convert.ToBase64String(result);
        }

        private static byte[] HashString(string password, byte[] salt)
        {
            return new Rfc2898DeriveBytes(password, salt, 10_000).GetBytes(PASSWORD_LENGTH);
        }

        public static bool IsPasswordMatch(string password, string hashAndSaltString)
        {
            if (password is null || hashAndSaltString is null)
            {
                if (hashAndSaltString is null && password is null)
                    return true;
                return false;
            }

            byte[] hashAndSalt = Convert.FromBase64String(hashAndSaltString);
            byte[] salt = new byte[SALT_LENGTH];
            Array.Copy(hashAndSalt, PASSWORD_LENGTH, salt, 0, SALT_LENGTH);
            byte[] newHash = HashString(password, salt);
            for (int i = 0; i < PASSWORD_LENGTH; i++)
                if (newHash[i] != hashAndSalt[i])
                    return false;
            return true;
        }
    }
}
