using brokenHeart.Models.Core.Abilities.Abilities;
using brokenHeart.Models.Core.Counters;
using brokenHeart.Models.Core.Modifiers.Effects;
using brokenHeart.Models.Core.Modifiers.Effects.Injuries;
using brokenHeart.Models.Core.Modifiers.Items;
using brokenHeart.Models.Core.Modifiers.Traits;
using brokenHeart.Models.Core.RoundReminders;
using brokenHeart.Models.Core.Stats;
using brokenHeart.Models.Utility;

namespace brokenHeart.Models.Core.Characters
{
    public class CharacterModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? DefaultShortcut { get; set; }
        public string Description { get; set; }

        //in m
        public decimal? Height { get; set; }

        //in kg
        public int? Weight { get; set; }

        //in €
        public decimal Money { get; set; }

        // Custom Currency
        public int C { get; set; }

        public int MaxHp { get; set; }

        public int Hp { get; set; }

        public int MaxTempHp { get; set; }
        public int TempHp { get; set; }

        //in m/Round
        public int MovementSpeed { get; set; }

        public int Armor { get; set; }
        public int Evasion { get; set; }

        public int? Age { get; set; }
        public string Notes { get; set; }
        public string Experience { get; set; }

        public List<StatValueModel> Stats { get; set; } = new List<StatValueModel>();
        public List<BodypartConditionModel> BodypartConditions { get; set; } =
            new List<BodypartConditionModel>();

        public List<VariableModel> Variables { get; set; } = new List<VariableModel>();

        public Children<AbilityModel> Abilities { get; set; } = new Children<AbilityModel>();

        public Children<ItemModel> Items { get; set; } = new Children<ItemModel>();

        public Children<TraitModel> Traits { get; set; } = new Children<TraitModel>();

        public Children<EffectModel> Effects { get; set; } = new Children<EffectModel>();

        public Children<InjuryEffectModel> InjuryEffects { get; set; } =
            new Children<InjuryEffectModel>();

        public Children<CounterModel> Counters { get; set; } = new Children<CounterModel>();

        public Children<RoundReminderModel> RoundReminders { get; set; } =
            new Children<RoundReminderModel>();

        public bool IsNPC { get; set; }
    }
}
