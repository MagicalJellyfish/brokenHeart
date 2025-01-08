using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace brokenHeart.Auth
{
    public static class AuthFunctions
    {
        public static ExecutionResult ValidatePasswordConstraints(string password)
        {
            if (password.Length < 6)
            {
                return new ExecutionResult()
                {
                    Succeeded = false,
                    Message = "Password must contain at least 6 characters"
                };
            }

            if (!password.Any(char.IsUpper))
            {
                return new ExecutionResult()
                {
                    Succeeded = false,
                    Message = "Password must contain an uppercase letter"
                };
            }

            if (!password.Any(char.IsLower))
            {
                return new ExecutionResult()
                {
                    Succeeded = false,
                    Message = "Password must contain a lowercase letter"
                };
            }

            if (!password.Any(char.IsDigit))
            {
                return new ExecutionResult()
                {
                    Succeeded = false,
                    Message = "Password must contain a number"
                };
            }

            return new ExecutionResult();
        }

        public static string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            return Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: saltBytes,
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: 300000,
                    numBytesRequested: 256 / 8
                )
            );
        }

        public static bool VerifyPassword(string hashedPassword, string givenPassword, string salt)
        {
            string hashedGivenPassword = HashPassword(givenPassword, salt);

            if (hashedPassword == hashedGivenPassword)
            {
                return true;
            }
            return false;
        }
    }
}
