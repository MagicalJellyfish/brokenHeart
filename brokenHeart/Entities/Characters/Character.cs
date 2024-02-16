using brokenHeart.Entities.Characters;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Items;
using brokenHeart.Entities.Effects;
using brokenHeart.Entities.Traits;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;
using brokenHeart.Entities.Effects.Injuries;

namespace brokenHeart.Entities
{
    public class Character
    {
        [JsonConstructor]
        public Character() { }
        public Character(string name, UserSimplified owner, int? age = null, string? defaultShortcut = null, string description = "", decimal? height = null, int? weight = null, decimal money = 0, string notes = "", string experience = "",
            List<Item>? inventory = null, List<Trait>? traits = null, List<Effect>? effects = null, int? hp = null)
        {
            Name = name;
            DefaultShortcut = defaultShortcut;
            Description = description;
            Height = height;
            Weight = weight;
            Money = money;
            Owner = owner;
            Age = age;
            Notes = notes;
            Experience = experience;
            Items = inventory ?? new List<Item>();
            Traits = traits ?? new List<Trait>();
            Effects = effects ?? new List<Effect>();

            BodypartConditions = new List<BodypartCondition>();
            foreach (Bodypart bp in Constants.Bodyparts.BaseBodyparts)
            {
                BodypartConditions.Add(new BodypartCondition(bp));
            }
            Stats = new List<StatValue>();
            foreach(Stat stat in Constants.Stats.stats)
            {
                Stats.Add(new StatValue(stat, 0));
            }

            Update();

            if(hp != null)
            {
                Hp = (int)hp;
            }
        }

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

        public int MaxHp { get; set; }

        public int Hp { get; set; }

        public int MaxTempHp { get; private set; }
        public int TempHp { get; set; }
        //in m/Round
        public int MovementSpeed { get; private set; }

        public int Armor { get; private set; }
        public int Evasion { get; private set; }

        public int? Age { get; set; }
        public string Notes { get; set; }
        public string Experience { get; set; }

        public virtual ICollection<StatValue> Stats { get; private set; } = new List<StatValue>();
        public virtual ICollection<BodypartCondition> BodypartConditions { get; set; } = new List<BodypartCondition>();

        [NotMapped]
        public ICollection<int>? ItemsIds { get; set; } = new List<int>();
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();

        [NotMapped]
        public ICollection<int>? TraitsIds { get; set; } = new List<int>();

        public virtual ICollection<Trait> Traits { get; set; } = new List<Trait>();

        [NotMapped]
        public ICollection<int>? EffectsIds { get; set; } = new List<int>();
        public virtual ICollection<Effect> Effects { get; set; } = new List<Effect>();

        [NotMapped]
        public ICollection<int>? InjuryEffectsIds { get; set; } = new List<int>();
        public virtual ICollection<InjuryEffect> InjuryEffects { get; set; } = new List<InjuryEffect>();

        [NotMapped]
        public ICollection<int>? CountersIds { get; set; } = new List<int>();
        public virtual ICollection<Counter> Counters { get; set; } = new List<Counter>();

        [NotMapped]
        public ICollection<int>? RoundRemindersIds { get; set; } = new List<int>();
        public virtual ICollection<RoundReminder> RoundReminders { get; set; } = new List<RoundReminder>();

        public byte[]? Image { get; set; }

        public bool IsNPC { get; set; }
        public virtual UserSimplified? Owner { get; set; }


        //Update any stat-derived changes
        public void Update()
        {
            //Reset
            int oldMaxHp = MaxHp;
            int oldMaxTempHp = MaxTempHp;
            int oldTempHp = TempHp;
            MaxHp = 0;
            MaxTempHp = 0;

            MovementSpeed = 10;

            Armor = 0;
            Evasion = 0;

            foreach (StatValue statValue in Stats)
            {
                statValue.Value = 0;
            }

            //TODO: BodypartConditions stuff

            //Add item modifiers
            foreach (var item in Items)
            {
                if(item.Equipped)
                {
                    UpdateModified(item);
                }
            }

            //Add trait modifiers
            foreach (var trait in Traits)
            {
                if(trait.Active)
                {
                    UpdateModified(trait);
                }
            }

            List<Effect> allEffects = Effects/*.Concat(InjuryEffects)*/.ToList();
            //Add effect modifiers
            foreach (var effect in allEffects)
            {
                UpdateModified(effect);
                MaxTempHp += effect.MaxTempHp;
            }

            //Add additional/missing maxHP to HP
            Hp += MaxHp - oldMaxHp;

            //Add/subtract values in temp Hp
            int maxDifference = MaxTempHp - oldMaxTempHp;
            int oldMissing = oldMaxTempHp - oldTempHp;
            if (maxDifference >= 0)
            {
                TempHp = oldTempHp + maxDifference;
            }
            else
            {
                int carryover = maxDifference + oldMissing;
                if (carryover < 0)
                {
                    TempHp = oldTempHp + carryover;
                }
                else
                {
                    TempHp = oldTempHp;
                }
            }
        }

        private void UpdateModified(Modifier modifier)
        {
            MaxHp += modifier.MaxHp;
            MovementSpeed += modifier.MovementSpeed;
            Armor += modifier.Armor;
            Evasion += modifier.Evasion;

            foreach(StatValue statIncrease in modifier.StatIncreases)
            {
                Stats.Single(x => statIncrease.Stat == x.Stat).Value += statIncrease.Value;
            }
        }
    }
}
