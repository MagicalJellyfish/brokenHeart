using brokenHeart.Models.Core.Abilities.Abilities;
using brokenHeart.Models.Core.Counters;
using brokenHeart.Models.Core.RoundReminders;
using brokenHeart.Models.Core.Stats;

namespace brokenHeart.Models.Core.Modifiers
{
    public class ModifierTemplateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Abstract { get; set; }
        public int MaxHp { get; set; }
        public int MovementSpeed { get; set; }
        public int Armor { get; set; }
        public int Evasion { get; set; }

        public List<StatValueModel> StatIncreases { get; set; } = new List<StatValueModel>();

        public List<CounterTemplateModel> CounterTemplates { get; set; } =
            new List<CounterTemplateModel>();

        public List<AbilityTemplateModel> AbilityTemplates { get; set; } =
            new List<AbilityTemplateModel>();

        public RoundReminderTemplateModel? RoundReminderTemplate { get; set; }
    }
}
