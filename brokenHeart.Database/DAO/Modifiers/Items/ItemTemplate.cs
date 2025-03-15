using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Characters;

namespace brokenHeart.Database.DAO.Modifiers.Items
{
    public class ItemTemplate : ModifierTemplate
    {
        [JsonConstructor]
        public ItemTemplate() { }

        public int Amount { get; set; } = 1;
        public string Unit { get; set; } = "";

        public ICollection<CharacterTemplate> CharacterTemplates { get; set; } =
            new List<CharacterTemplate>();
    }
}
