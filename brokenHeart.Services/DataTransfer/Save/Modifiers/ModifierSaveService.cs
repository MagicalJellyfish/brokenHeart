using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Models.DataTransfer.Save.ElementFields.Modifiers;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers
{
    internal class ModifierSaveService : IModifierSaveService
    {
        public void UpdateGivenModifier(Modifier modifier, List<ElementUpdate> updates)
        {
            foreach (ElementUpdate update in updates)
            {
                switch ((ModifierField)update.FieldId)
                {
                    case ModifierField.Name:
                        modifier.Name = update.Value;
                        break;
                    case ModifierField.Abstract:
                        modifier.Abstract = update.Value;
                        break;
                    case ModifierField.Description:
                        modifier.Description = update.Value;
                        break;
                    case ModifierField.MaxHp:
                        modifier.MaxHp = update.Value.SafeParseInt();
                        break;
                    case ModifierField.MovementSpeed:
                        modifier.MovementSpeed = update.Value.SafeParseInt();
                        break;
                    case ModifierField.Armor:
                        modifier.Armor = update.Value.SafeParseInt();
                        break;
                    case ModifierField.Evasion:
                        modifier.Evasion = update.Value.SafeParseInt();
                        break;
                }
            }
        }
    }
}
