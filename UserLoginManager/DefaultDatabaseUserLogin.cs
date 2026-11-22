using System.Security.Cryptography;
using System.Text;

namespace MultiplayerARPG.MMO
{
    /// <summary>
    /// New passwords are stored as "p1$salt$hash": PBKDF2-SHA512 with a random per-user salt (the optional
    /// prefix/postfix settings act as a server-wide pepper). Legacy unsalted-MD5 hashes still verify so
    /// existing accounts can log in; <see cref="NeedsRehash"/> flags them so the database upgrades them.
    /// </summary>
    public class DefaultDatabaseUserLogin : IDatabaseUserLogin
    {
        private string _passwordSaltPrefix = string.Empty;
        private string _passwordSaltPostfix = string.Empty;

        public DefaultDatabaseUserLogin(DefaultDatabaseUserLoginConfig config)
        {
            _passwordSaltPrefix = config.PasswordSaltPrefix;
            _passwordSaltPostfix = config.PasswordSaltPostfix;
        }

        public string GenerateNewId()
        {
            return System.Guid.NewGuid().ToString("N");
        }

        private const string Version = "p1$";
        private const int Iterations = 100000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public string GetHashedPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);
            return Version + ToB64(salt) + "$" + ToB64(Pbkdf2(password, salt));
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
                return false;
            byte[] actual;
            byte[] expected;
            if (hashedPassword.StartsWith(Version))
            {
                string[] parts = hashedPassword.Substring(Version.Length).Split('$');
                if (parts.Length != 2)
                    return false;
                try
                {
                    actual = Pbkdf2(password, FromB64(parts[0]));
                    expected = FromB64(parts[1]);
                }
                catch (System.FormatException)
                {
                    return false;
                }
            }
            else
            {
                actual = Encoding.UTF8.GetBytes(GetMD5(password));
                expected = Encoding.UTF8.GetBytes(hashedPassword.ToLowerInvariant());
            }
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        public bool NeedsRehash(string hashedPassword)
        {
            return !hashedPassword.StartsWith(Version);
        }

        private byte[] Pbkdf2(string password, byte[] salt)
        {
            using (Rfc2898DeriveBytes kdf = new Rfc2898DeriveBytes(_passwordSaltPrefix + password + _passwordSaltPostfix, salt, Iterations, HashAlgorithmName.SHA512))
                return kdf.GetBytes(HashSize);
        }

        // Unpadded base64: keeps the whole hash at 69 chars, inside the MySQL varchar(72) column.
        private static string ToB64(byte[] data) => System.Convert.ToBase64String(data).TrimEnd('=');

        private static byte[] FromB64(string text) => System.Convert.FromBase64String(text + new string('=', (4 - text.Length % 4) % 4));

        private string GetMD5(string text)
        {
            if (!string.IsNullOrEmpty(_passwordSaltPrefix))
                text = _passwordSaltPrefix + text;
            if (!string.IsNullOrEmpty(_passwordSaltPostfix))
                text = text + _passwordSaltPostfix;

            // byte array representation of that string
            byte[] encodedPassword = new UTF8Encoding().GetBytes(text);

            // need MD5 to calculate the hash
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

            // string representation (similar to UNIX format)
            return System.BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }
    }
}
