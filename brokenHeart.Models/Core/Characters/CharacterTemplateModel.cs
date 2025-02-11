using brokenHeart.Models.Core.Abilities.Abilities;
using brokenHeart.Models.Core.Counters;
using brokenHeart.Models.Core.Modifiers.Effects;
using brokenHeart.Models.Core.Modifiers.Items;
using brokenHeart.Models.Core.Modifiers.Traits;
using brokenHeart.Models.Core.RoundReminders;
using brokenHeart.Models.Utility;

namespace brokenHeart.Models.Core.Characters
{
    public class CharacterTemplateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //in m
        public decimal? Height { get; set; }

        //in kg
        public int? Weight { get; set; }

        //in €
        public decimal Money { get; set; }

        public int? Age { get; set; }
        public string Notes { get; set; }
        public string Experience { get; set; }

        public List<VariableModel> Variables { get; set; } = new List<VariableModel>();

        public Children<AbilityTemplateModel> AbilityTemplates { get; set; } =
            new Children<AbilityTemplateModel>();

        public Children<ItemTemplateModel> ItemTemplates { get; set; } =
            new Children<ItemTemplateModel>();

        public Children<TraitTemplateModel> TraitTemplates { get; set; } =
            new Children<TraitTemplateModel>();

        public Children<EffectTemplateModel> EffectTemplates { get; set; } =
            new Children<EffectTemplateModel>();

        public Children<CounterTemplateModel> CounterTemplates { get; set; } =
            new Children<CounterTemplateModel>();

        public Children<RoundReminderTemplateModel> RoundReminderTemplates { get; set; } =
            new Children<RoundReminderTemplateModel>();

        public byte[]? Image { get; set; }

        public bool IsNPC { get; set; }
    }
}
