using System.Text.Json.Serialization;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;

namespace brokenHeart.Database.DAO.Modifiers.Effects.Injuries
{
    public class InjuryEffectTemplate : EffectTemplate
    {
        [JsonConstructor]
        public InjuryEffectTemplate() { }

        public InjuryEffectTemplate(
            string name,
            string @abstract,
            string duration,
            int bodypartId,
            InjuryLevel injuryLevel,
            string description = "",
            int maxHp = 0,
            int movementSpeed = 0,
            int armor = 0,
            int evasion = 0,
            string hp = "",
            int maxTempHp = 0,
            List<StatValue>? statIncreases = null,
            List<AbilityTemplate>? abilityTemplates = null,
            List<CounterTemplate>? counterTemplates = null,
            EffectCounterTemplate? effectCounterTemplate = null,
            RoundReminderTemplate? reminderTemplate = null
        )
            : base(
                name,
                @abstract,
                duration,
                description,
                maxHp,
                movementSpeed,
                armor,
                evasion,
                hp,
                maxTempHp,
                statIncreases,
                abilityTemplates,
                counterTemplates,
                effectCounterTemplate,
                reminderTemplate
            )
        {
            BodypartId = bodypartId;
            InjuryLevel = injuryLevel;
        }

        public InjuryLevel InjuryLevel { get; set; }

        public int BodypartId { get; set; }
        public Bodypart Bodypart { get; set; }

        public new InjuryEffect Instantiate()
        {
            return new InjuryEffect(
                Name,
                Abstract,
                Duration,
                Bodypart,
                InjuryLevel,
                Description,
                MaxHp,
                MovementSpeed,
                Armor,
                Evasion,
                Hp,
                MaxTempHp,
                StatIncreases.Select(x => x.Instantiate()).ToList(),
                AbilityTemplates.Select(x => x.Instantiate()).ToList(),
                CounterTemplates.Select(x => x.Instantiate()).ToList(),
                RoundReminderTemplate?.Instantiate(),
                EffectCounterTemplate?.Instantiate()
            );
        }
    }
}
