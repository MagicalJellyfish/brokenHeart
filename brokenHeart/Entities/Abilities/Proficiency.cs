using System.Text.Json.Serialization;
using brokenHeart.Entities.Abilities.Abilities.TargetAbilities.TargetAttackAbilities;

namespace brokenHeart.Entities.Abilities
{
    public class Proficiency
    {
        [JsonConstructor]
        public Proficiency() { }
        public Proficiency(AbilityType abilityType, int value)
        {
            AbilityType = abilityType;
            Value = value;
        }

        public int Id { get; set; }
        public int Value { get; set; }

        public int AbilityTypeId { get; set; }
        public virtual AbilityType AbilityType { get; set; }
    }
}
