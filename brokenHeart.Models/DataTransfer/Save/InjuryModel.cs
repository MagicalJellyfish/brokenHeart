using brokenHeart.Database.DAO.Characters;

namespace brokenHeart.Models.DataTransfer.Save
{
    public class InjuryModel
    {
        public int Bodypart { get; set; }
        public InjuryLevel InjuryLevel { get; set; }
    }
}
