using System.ComponentModel.DataAnnotations;

namespace brokenHeart.Auth
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Discord User Id is required")]
        public ulong? DiscordId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
