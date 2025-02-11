using brokenHeart.Database.DAO.Characters;

namespace brokenHeart.Models.Core.Characters
{
    public class BodypartConditionModel
    {
        public int Id { get; set; }
        public BodypartModel Bodypart { get; set; }

        public InjuryLevel InjuryLevel { get; set; } = InjuryLevel.None;
    }
}
