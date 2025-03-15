using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Models.DataTransfer.Save;

namespace brokenHeart.Services.DataTransfer.Save.Entities.Modifiers
{
    public interface IModifierTemplateSaveService
    {
        public void UpdateGivenModifierTemplate(
            ModifierTemplate modifierTemplate,
            List<ElementUpdate> updates
        );
    }
}
