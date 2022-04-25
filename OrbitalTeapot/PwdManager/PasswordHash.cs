using System;
using System.Collections.Generic;
using System.Text;

namespace OrbitalTeapot.PwdManager
{
    public static class PasswordHashStatic
    {
        /// <summary>
        /// Creates passwordHash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns>bool</returns>
        /// <exception cref="Exception"></exception>
        public static bool CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            passwordHash = null;
            passwordSalt = null;
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Cannot create password hash. Password is empty or whitespace.");
            }

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return true;
        }

        /// <summary>
        /// Verify passwordHash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="storedHash"></param>
        /// <param name="storedSalt"></param>
        /// <returns>bool</returns>
        /// <exception cref="Exception"></exception>
        public static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Cannot verify password hash. Password is empty or whitespace.");
            }

            if (storedHash.Length != 64 || storedSalt.Length != 128)
            {
                throw new Exception("Cannot verify password hash. Hash or salt of invalid size");
            }

            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i])
                {
                    throw new Exception("Password verification failed");
                }
            }
            return true;
        }
    }

    public interface IPasswordManagerService
    {
        bool CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }

    public class PasswordHash : IPasswordManagerService
    {
        /// <summary>
        /// Creates passwordHash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns>bool</returns>
        /// <exception cref="Exception"></exception>
        public bool CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            passwordHash = null;
            passwordSalt = null;
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Cannot create password hash. Password is empty or whitespace.");
            }

            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            return true;
        }

        /// <summary>
        /// Verify passwordHash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="storedHash"></param>
        /// <param name="storedSalt"></param>
        /// <returns>bool</returns>
        /// <exception cref="Exception"></exception>
        public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Cannot verify password hash. Password is empty or whitespace.");
            }

            if (storedHash.Length != 64 || storedSalt.Length != 128)
            {
                throw new Exception("Cannot verify password hash. Hash or salt of invalid size");
            }

            using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i])
                {
                    throw new Exception("Password verification failed");
                }
            }
            return true;
        }
    }
}
