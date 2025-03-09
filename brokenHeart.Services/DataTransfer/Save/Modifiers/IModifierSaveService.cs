using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers
{
    public interface IModifierSaveService
    {
        public void UpdateStats(int id, List<StatValueModel> stats);

        public void UpdateGivenModifier(Modifier modifier, List<ElementUpdate> updates);
    }
}
