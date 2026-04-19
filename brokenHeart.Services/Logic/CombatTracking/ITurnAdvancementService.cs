using brokenHeart.Database.DAO.Combat;
using brokenHeart.Models.brokenHand;

namespace brokenHeart.Services.Logic.CombatTracking
{
    public interface ITurnAdvancementService
    {
        public List<Message> AdvanceTurn(Combat combat, List<CombatEntry> orderedEntries);
    }
}
