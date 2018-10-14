using SimplePassword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASP_MEMBER_WORKSHOP.Models
{
    public class PasswordHashModel
    {

        // Hash password method
        public static string Hash(string password)
        {
            var saltedPasswordHash = new SaltedPasswordHash(password, 8);
            return saltedPasswordHash.Hash + ":" + saltedPasswordHash.Salt;
        }

        // Verify password method
        public static bool Verify(string password, string passwordHash)
        {
            string[] passwordHashes = passwordHash.Split(':');
            if (passwordHashes.Length == 2)
            {
                var saltedPasswordHash = new SaltedPasswordHash(passwordHashes[0], passwordHashes[1]);
                return saltedPasswordHash.Verify(password);
            }
            return false;
        }
    }
}