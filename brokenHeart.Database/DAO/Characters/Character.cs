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
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO
{
    public class Character : IDao
    {
        [JsonConstructor]
        public Character() { }

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

        public ICollection<StatValue> Stats { get; private set; } = new List<StatValue>();
        public ICollection<BodypartCondition> BodypartConditions { get; set; } =
            new List<BodypartCondition>();

        public ICollection<Variable> Variables { get; set; } = new List<Variable>();

        public ICollection<Ability> Abilities { get; set; } = new List<Ability>();

        public ICollection<Item> Items { get; set; } = new List<Item>();

        public ICollection<Trait> Traits { get; set; } = new List<Trait>();

        public ICollection<Effect> Effects { get; set; } = new List<Effect>();

        public ICollection<InjuryEffect> InjuryEffects { get; set; } = new List<InjuryEffect>();

        public ICollection<Counter> Counters { get; set; } = new List<Counter>();

        public int? DeathCounterId { get; set; }
        public Counter? DeathCounter { get; set; }

        public ICollection<RoundReminder> RoundReminders { get; set; } = new List<RoundReminder>();

        public bool IsNPC { get; set; }

        public int OwnerId { get; set; }
        public UserSimplified? Owner { get; set; }

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
