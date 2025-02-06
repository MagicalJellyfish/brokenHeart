using brokenHeart.Models;

namespace brokenHeart.Authentication.Services
{
    public interface IPasswordService
    {
        public ExecutionResult ValidatePasswordConstraints(string password);
        public string HashPassword(string password, string salt);
        public bool VerifyPassword(string hashedPassword, string givenPassword, string salt);
    }
}
