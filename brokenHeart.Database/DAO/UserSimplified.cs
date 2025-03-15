using System.Text.Json.Serialization;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO
{
    public class UserSimplified : IDao
    {
        [JsonConstructor]
        public UserSimplified() { }

        public int Id { get; set; }
        public string Username { get; set; }
        public ulong DiscordId { get; set; }

        public string? DefaultAbilityString { get; set; }
        public string? DefaultTargetString { get; set; }

        public Character? ActiveCharacter { get; set; }
        public ICollection<Character> Characters { get; set; } = new List<Character>();
    }
}
