using System.Text.Json.Serialization;
using brokenHeart.Database.Interfaces;

namespace brokenHeart.Database.DAO.Characters
{
    public class BodypartCondition : IDao
    {
        [JsonConstructor]
        public BodypartCondition() { }

        public BodypartCondition(Bodypart bodypart)
        {
            Bodypart = bodypart;
        }

        public int Id { get; set; }

        public int BodypartId { get; set; }
        public virtual Bodypart Bodypart { get; set; }

        public virtual Character Character { get; set; }

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
