using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Abilities.Abilities.TargetAbilities.TargetAttackAbilities
{
    public class AbilityType
    {
        [JsonConstructor]
        public AbilityType() { }
        public AbilityType(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
