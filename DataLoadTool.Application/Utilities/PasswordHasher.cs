using DataLoadTool.Core.Models;
using System.Security.Cryptography;
using System.Text;

namespace DataLoadTool.Application.Utilities
{
    public static class PasswordHasher
    {
        private static readonly int KeySize = 64; // 64 bytes
        private static readonly int Iterations = 350000;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512; // hashing algorithm (SHA-512)

        // https://code-maze.com/csharp-hashing-salting-passwords-best-practices

        public static HashedPasswordResult GenerateHashedPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.");
            }

            var salt = RandomNumberGenerator.GetBytes(KeySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, Iterations, HashAlgorithm, KeySize);

            return new HashedPasswordResult
            {
                Hash = Convert.ToHexString(hash),
                Salt = Convert.ToHexString(salt)
            };
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password is required.");
            }

            if (string.IsNullOrWhiteSpace(storedHash))
            {
                throw new ArgumentException("Hash is required.");
            }

            if (string.IsNullOrWhiteSpace(storedSalt))
            {
                throw new ArgumentException("Salt is required.");
            }

            try
            {
                var saltBytes = Convert.FromHexString(storedSalt);
                var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, HashAlgorithm, KeySize);
                return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(storedHash));
            }
            catch (FormatException)
            {
                throw new ArgumentException("Stored hash or salt is in an invalid format.");
            }
        }
    }
}
