using System.ComponentModel.DataAnnotations;

namespace brokenHeart.Authentication.Models
{
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
}
