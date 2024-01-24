using brokenHeart.Entities.Characters;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities
{
    public class UserSimplified
    {
        [JsonConstructor]
        public UserSimplified() { }
        public UserSimplified(string username)
        {
            Username = username;
        }

        public int Id { get; set; }
        public string Username { get; set; }

        public virtual ICollection<PlayerCharacter> Characters { get; set; } = new List<PlayerCharacter>();
    }
}
