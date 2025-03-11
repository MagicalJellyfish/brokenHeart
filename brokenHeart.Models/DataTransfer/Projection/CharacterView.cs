using brokenHeart.Database.DAO.Characters;

namespace brokenHeart.Models.DataTransfer.Projection
{
    public class CharacterView
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsNPC { get; set; }
        public string? DefaultShortcut { get; set; }

        public int? Age { get; set; }
        public decimal? Height { get; set; }
        public int? Weight { get; set; }

        public int MovementSpeed { get; set; }
        public int Evasion { get; set; }
        public int Armor { get; set; }
        public int Defense { get; set; }

        public string Experience { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }

        public decimal Money { get; set; }
        public int C { get; set; }

        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public int TempHp { get; set; }
        public int MaxTempHp { get; set; }

        public DeathCounterModel DeathCounter { get; set; }

        public List<StatModel> Stats { get; set; }

        public List<HpImpactModel> HpImpacts { get; set; }

        public List<InjuryModel> Injuries { get; set; }

        public List<ElementList> ElementLists { get; set; }

        public class DeathCounterModel
        {
            public int Id { get; set; }
            public int ValueFieldId { get; set; }
            public int Value { get; set; }
            public int Max { get; set; }
        }

        public class StatModel
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        public class HpImpactModel
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class InjuryModel
        {
            public int Bodypart { get; set; }
            public InjuryLevel InjuryLevel { get; set; }
        }

        public abstract class ListItem
        {
            public int Id { get; set; }
            public int ViewPosition { get; set; }
        }

        public abstract class SourcedListItem : ListItem
        {
            public string Source { get; set; }
        }

        public class AbilityModel : SourcedListItem
        {
            public string Name { get; set; }
            public string Abstract { get; set; }
            public int? Uses { get; set; }
            public int? MaxUses { get; set; }
        }

        public class TraitModel : ListItem
        {
            public string Name { get; set; }
            public string Abstract { get; set; }
            public bool Active { get; set; }
        }

        public class ItemModel : ListItem
        {
            public string Name { get; set; }
            public string Abstract { get; set; }
            public bool Equipped { get; set; }
            public int Amount { get; set; }
        }

        public class EffectModel : ListItem
        {
            public string Name { get; set; }
            public string Abstract { get; set; }
        }

        public class CounterModel : SourcedListItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int Value { get; set; }
            public int Max { get; set; }
        }

        public class ReminderModel : SourcedListItem
        {
            public string Reminder { get; set; }
            public bool Reminding { get; set; }
        }

        public class VariableModel : ListItem
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
