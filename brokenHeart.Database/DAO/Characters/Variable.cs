using System.Text.Json.Serialization;

namespace brokenHeart.Database.DAO.Characters
{
    public class Variable
    {
        [JsonConstructor]
        public Variable() { }

        public Variable(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public int Id { get; set; }
        public string Name { get; set; } = "New Variable";
        public int Value { get; set; } = 0;

        public int? CharacterId { get; set; }
        public virtual Character? Character { get; set; }

        public int? CharacterTemplateId { get; set; }
        public CharacterTemplate? CharacterTemplate { get; set; }

        public int ViewPosition { get; set; }

        public Variable Instantiate()
        {
            return new Variable(Name, Value);
        }
    }
}
