using brokenHeart.Entities.Abilities.Abilities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Abilities
{
    public class Roll
    {
        [JsonConstructor]
        public Roll() { }

        public Roll(string name, string instruction)
        {
            Name = name;
            Instruction = instruction;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Instruction { get; set; }

        public int? AbilityId { get; set; }
        public Ability? Ability { get; set; }
        
        public int? AbilityTemplateId { get; set; }
        public AbilityTemplate? AbilityTemplate { get; set; }

        public Roll Instantiate()
        {
            return new Roll(Name, Instruction);
        }
    }
}
