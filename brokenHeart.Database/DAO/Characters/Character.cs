using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Database.DAO.Modifiers.Effects.Injuries;
using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;

namespace brokenHeart.Database.DAO
{
    public class Character
    {
        [JsonConstructor]
        public Character() { }

        public Character(
            string name,
            UserSimplified owner,
            int? age = null,
            string? defaultShortcut = null,
            string description = "",
            decimal? height = null,
            int? weight = null,
            decimal money = 0,
            string notes = "",
            string experience = "",
            List<Variable>? variables = null,
            List<Item>? inventory = null,
            List<Trait>? traits = null,
            List<Effect>? effects = null,
            List<Ability>? abilities = null,
            int? hp = null,
            bool isNPC = false
        )
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
            IsNPC = isNPC;
            Variables = variables ?? new List<Variable>();
            Items = inventory ?? new List<Item>();
            Traits = traits ?? new List<Trait>();
            Effects = effects ?? new List<Effect>();
            Abilities = abilities ?? new List<Ability>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = "New Character";
        public string? DefaultShortcut { get; set; }
        public string Description { get; set; } = "";

        //in m
        public decimal? Height { get; set; }

        //in kg
        public int? Weight { get; set; }

        //in €
        public decimal Money { get; set; } = 0;

        // Custom Currency
        public int C { get; set; } = 0;

        public int MaxHp { get; set; }

        public int Hp { get; set; }

        public int MaxTempHp { get; private set; }
        public int TempHp { get; set; }

        //in m/Round
        public int MovementSpeed { get; private set; }

        public int Armor { get; private set; }
        public int Evasion { get; private set; }

        public int? Age { get; set; }
        public string Notes { get; set; } = "";
        public string Experience { get; set; } = "";

        public virtual ICollection<StatValue> Stats { get; private set; } = new List<StatValue>();
        public virtual ICollection<BodypartCondition> BodypartConditions { get; set; } =
            new List<BodypartCondition>();

        [NotMapped]
        public ICollection<int>? VariablesIds { get; set; } = new List<int>();
        public virtual ICollection<Variable> Variables { get; set; } = new List<Variable>();

        [NotMapped]
        public ICollection<int>? AbilitiesIds { get; set; } = new List<int>();
        public virtual ICollection<Ability> Abilities { get; set; } = new List<Ability>();

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
        public virtual ICollection<InjuryEffect> InjuryEffects { get; set; } =
            new List<InjuryEffect>();

        [NotMapped]
        public ICollection<int>? CountersIds { get; set; } = new List<int>();
        public virtual ICollection<Counter> Counters { get; set; } = new List<Counter>();

        public int? DeathCounterId { get; set; }
        public virtual Counter? DeathCounter { get; set; }

        [NotMapped]
        public ICollection<int>? RoundRemindersIds { get; set; } = new List<int>();
        public virtual ICollection<RoundReminder> RoundReminders { get; set; } =
            new List<RoundReminder>();

        public bool IsNPC { get; set; }

        public int OwnerId { get; set; }
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

            //Add item modifiers
            foreach (var item in Items)
            {
                if (item.Equipped)
                {
                    for (int i = 0; i < item.Amount; i++)
                    {
                        UpdateModified(item);
                    }
                }
            }

            //Add trait modifiers
            foreach (var trait in Traits)
            {
                if (trait.Active)
                {
                    UpdateModified(trait);
                }
            }

            //Add effect modifiers
            foreach (var effect in Effects)
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

            foreach (StatValue statIncrease in modifier.StatIncreases)
            {
                Stats.Single(x => statIncrease.StatId == x.StatId).Value += statIncrease.Value;
            }
        }

        public void ShortRest()
        {
            AbilityReplenish(ReplenishType.ShortRest);
        }

        public void LongRest()
        {
            if (Hp < MaxHp)
            {
                if (Hp > (MaxHp / 2))
                {
                    Hp = MaxHp;
                }
                else
                {
                    Hp += (MaxHp / 2);
                }
            }

            AbilityReplenish(ReplenishType.LongRest);
        }

        private void AbilityReplenish(ReplenishType replenishType)
        {
            List<Ability> allAbilities = Abilities
                .Concat(Traits.SelectMany(x => x.Abilities))
                .Concat(Items.SelectMany(x => x.Abilities))
                .Concat(Effects.SelectMany(x => x.Abilities))
                .ToList();

            foreach (Ability ability in allAbilities)
            {
                if (ability.ReplenishType <= replenishType)
                {
                    ability.Uses = ability.MaxUses;
                }
            }
        }

        public List<Counter> GetAllCounters()
        {
            List<Counter?> counterList = new List<Counter?>();

            counterList.AddRange(Counters);

            counterList.AddRange(Items.SelectMany(x => x.Counters));

            counterList.AddRange(Effects.SelectMany(x => x.Counters));
            counterList.AddRange(Effects.Select(x => x.EffectCounter));

            counterList.AddRange(Traits.SelectMany(x => x.Counters));

            return counterList.Where(x => x != null).ToList();
        }
    }
}
