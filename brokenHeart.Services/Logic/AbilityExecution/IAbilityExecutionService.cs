using brokenHeart.Models.brokenHand;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Services.Logic.AbilityExecution
{
    public interface IAbilityExecutionService
    {
        public ActionResult<List<Message>> ExecuteAbility(
            int charId,
            string shortcut,
            string? targets,
            string? selfModifer,
            string? targetModifier,
            string? damageModifier
        );
    }
}
