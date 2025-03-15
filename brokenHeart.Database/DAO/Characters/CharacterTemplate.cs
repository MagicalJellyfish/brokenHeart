using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Characters
{
    public class CharacterTemplate : IDao
    {
        [JsonConstructor]
        public CharacterTemplate() { }

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

        public virtual ICollection<Variable> Variables { get; set; } = new List<Variable>();

        public virtual ICollection<AbilityTemplate> AbilityTemplates { get; set; } =
            new List<AbilityTemplate>();

        public virtual ICollection<ItemTemplate> ItemTemplates { get; set; } =
            new List<ItemTemplate>();

        public virtual ICollection<TraitTemplate> TraitTemplates { get; set; } =
            new List<TraitTemplate>();

        public virtual ICollection<EffectTemplate> EffectTemplates { get; set; } =
            new List<EffectTemplate>();

        public virtual ICollection<CounterTemplate> CounterTemplates { get; set; } =
            new List<CounterTemplate>();

        public virtual ICollection<RoundReminderTemplate> RoundReminderTemplates { get; set; } =
            new List<RoundReminderTemplate>();

        public byte[]? Image { get; set; }

        public bool IsNPC { get; set; }
    }
}
