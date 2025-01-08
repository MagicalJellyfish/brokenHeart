using System.ComponentModel.DataAnnotations;

namespace brokenHeart.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username is required!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }
    }

    public class RegistrationModel
    {
        [Required(ErrorMessage = "Username is required!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Discord ID is required!")]
        public ulong DiscordId { get; set; }

        [Required(ErrorMessage = "Registration Token is required!")]
        public string RegistrationToken { get; set; }
    }

    public class RefreshModel
    {
        [Required(ErrorMessage = "Access Token is required!")]
        public string AccessToken { get; set; }

        [Required(ErrorMessage = "Registration Token is required!")]
        public string RefreshToken { get; set; }
    }
}
