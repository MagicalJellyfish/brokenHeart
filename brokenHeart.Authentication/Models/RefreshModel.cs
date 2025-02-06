using System.ComponentModel.DataAnnotations;

namespace brokenHeart.Authentication.Models
{
    public class RefreshModel
    {
        [Required(ErrorMessage = "Access Token is required!")]
        public string AccessToken { get; set; }

        [Required(ErrorMessage = "Registration Token is required!")]
        public string RefreshToken { get; set; }
    }
}
