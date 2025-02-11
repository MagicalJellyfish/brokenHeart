using brokenHeart.Models.Core.Characters;

namespace brokenHeart.Models.Core
{
    public class UserSimplifiedModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public ulong DiscordId { get; set; }

        public string? DefaultAbilityString { get; set; }
        public string? DefaultTargetString { get; set; }

        public CharacterModel? ActiveCharacter { get; set; }
        public List<CharacterModel> Characters { get; set; } = new List<CharacterModel>();
    }
}
