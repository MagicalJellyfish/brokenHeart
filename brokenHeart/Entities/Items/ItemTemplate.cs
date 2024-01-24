using brokenHeart.Entities.Counters;
using System.Text.Json.Serialization;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;

namespace brokenHeart.Entities.Items
{
    public class ItemTemplate : ModifierTemplate
    {
        [JsonConstructor]
        public ItemTemplate() { }
        public ItemTemplate(string name, string description = "", string @abstract = "", int maxHp = 0, int movementSpeed = 0, int armor = 0, int evasion = 0,
            List<StatValue>? statIncreases = null, List<CounterTemplate>? counterTemplates = null, 
            RoundReminderTemplate? reminderTemplate = null)
            : base(name, @abstract, description, maxHp, movementSpeed, armor, evasion, statIncreases, counterTemplates, reminderTemplate)
        { }


        public Item Instantiate()
        {
            return new Item(Name, Abstract, Description, MaxHp, MovementSpeed, Armor, Evasion,
                StatIncreases.ToList(), CounterTemplates.Select(x => x.Instantiate()).ToList(), 
                RoundReminderTemplate?.Instantiate());
        }
    }
}
