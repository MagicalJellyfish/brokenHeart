using System.Text.Json.Serialization;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Characters
{
    public class BodypartCondition : IDao
    {
        [JsonConstructor]
        public BodypartCondition() { }

        public int Id { get; set; }

        public int BodypartId { get; set; }
        public Bodypart Bodypart { get; set; }

        public Character Character { get; set; }

        public InjuryLevel InjuryLevel { get; set; } = InjuryLevel.None;
    }

    public enum InjuryLevel
    {
        None,
        Minor,
        Medium,
        Major,
        Dismember
    }
}
