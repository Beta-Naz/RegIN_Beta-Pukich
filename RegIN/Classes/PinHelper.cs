using System;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace RegIN.Classes
{
    public static class PinHelper
    {
        public static bool IsValid(string pin)
        {
            return Regex.IsMatch(pin ?? "", @"\A[0-9]{4}\z");
        }

        public static string Hash(string pin)
        {
            byte[] salt = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 =
                new Rfc2898DeriveBytes(pin, salt, 200000))
            {
                return Convert.ToBase64String(salt) + ":" +
                    Convert.ToBase64String(pbkdf2.GetBytes(32));
            }
        }

        public static bool Verify(string pin, string savedHash)
        {
            if (!IsValid(pin) || string.IsNullOrEmpty(savedHash))
                return false;

            string[] parts = savedHash.Split(':');

            if (parts.Length != 2)
                return false;

            byte[] salt = Convert.FromBase64String(parts[0]);

            using (var pbkdf2 =
                new Rfc2898DeriveBytes(pin, salt, 200000))
            {
                return Convert.ToBase64String(
                    pbkdf2.GetBytes(32)) == parts[1];
            }
        }
    }
}