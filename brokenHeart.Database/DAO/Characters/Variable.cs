using System.Text.Json.Serialization;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Characters
{
    public class Variable : IDao, IElement
    {
        [JsonConstructor]
        public Variable() { }

        public int Id { get; set; }
        public string Name { get; set; } = "New Variable";
        public int Value { get; set; } = 0;

        public int? CharacterId { get; set; }
        public Character? Character { get; set; }

        public int? CharacterTemplateId { get; set; }
        public CharacterTemplate? CharacterTemplate { get; set; }

        public int ViewPosition { get; set; }
    }
}
