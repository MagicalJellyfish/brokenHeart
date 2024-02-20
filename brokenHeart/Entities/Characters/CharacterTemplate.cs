using brokenHeart.DB;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Effects;
using brokenHeart.Entities.Items;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Traits;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace brokenHeart.Entities.Characters
{
    public class CharacterTemplate
    {
        [JsonConstructor]
        public CharacterTemplate() { }
        public CharacterTemplate(string name, int age, string description = "", decimal height = 0, int weight = 0, decimal money = 0, string notes = "", string experience = "",
            List<ItemTemplate>? inventory = null, List<TraitTemplate>? traits = null)
        {
            Name = name;
            Description = description;
            Height = height;
            Weight = weight;
            Money = money;
            Age = age;
            Notes = notes;
            Experience = experience;
            ItemTemplates = inventory ?? new List<ItemTemplate>();
            TraitTemplates = traits ?? new List<TraitTemplate>();
        }

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


        [NotMapped]
        public ICollection<int>? ItemTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<ItemTemplate> ItemTemplates { get; set; } = new List<ItemTemplate>();

        [NotMapped]
        public ICollection<int>? TraitTemplatesIds { get; set; } = new List<int>();

        public virtual ICollection<TraitTemplate> TraitTemplates { get; set; } = new List<TraitTemplate>();

        [NotMapped]
        public ICollection<int>? EffectTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<EffectTemplate> EffectTemplates { get; set; } = new List<EffectTemplate>();

        [NotMapped]
        public ICollection<int>? CounterTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<CounterTemplate> CounterTemplates { get; set; } = new List<CounterTemplate>();

        [NotMapped]
        public ICollection<int>? RoundReminderTemplatesIds { get; set; } = new List<int>();
        public virtual ICollection<RoundReminderTemplate> RoundReminderTemplates { get; set; } = new List<RoundReminderTemplate>();

        public byte[]? Image { get; set; }

        public bool IsNPC { get; set; }

        public Character Instantiate(UserSimplified owner)
        {
            return new Character(Name, owner, Age, null, Description, Height, Weight, Money, Notes, Experience, 
                ItemTemplates.Select(x => x.Instantiate()).ToList(), TraitTemplates.Select(x => x.Instantiate()).ToList(), EffectTemplates.Select(x => x.Instantiate()).ToList());
        }
    }
}
