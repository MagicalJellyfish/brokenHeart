using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Characters;

namespace brokenHeart.Database.DAO.Modifiers.Traits
{
    public class TraitTemplate : ModifierTemplate
    {
        [JsonConstructor]
        public TraitTemplate() { }

        public ICollection<CharacterTemplate> CharacterTemplates { get; set; } =
            new List<CharacterTemplate>();
    }
}
