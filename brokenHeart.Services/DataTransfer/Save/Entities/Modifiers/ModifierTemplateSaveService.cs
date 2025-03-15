using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Entities.Modifiers
{
    internal class ModifierTemplateSaveService : IModifierTemplateSaveService
    {
        private readonly BrokenDbContext _context;

        public ModifierTemplateSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public void UpdateGivenModifierTemplate(
            ModifierTemplate modifierTemplate,
            List<ElementUpdate> updates
        )
        {
            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(ModifierTemplate.Name):
                        modifierTemplate.Name = update.Value;
                        break;
                    case nameof(ModifierTemplate.Abstract):
                        modifierTemplate.Abstract = update.Value;
                        break;
                    case nameof(ModifierTemplate.Description):
                        modifierTemplate.Description = update.Value;
                        break;
                    case nameof(ModifierTemplate.MaxHp):
                        modifierTemplate.MaxHp = update.Value.SafeParseInt();
                        break;
                    case nameof(ModifierTemplate.MovementSpeed):
                        modifierTemplate.MovementSpeed = update.Value.SafeParseInt();
                        break;
                    case nameof(ModifierTemplate.Armor):
                        modifierTemplate.Armor = update.Value.SafeParseInt();
                        break;
                    case nameof(ModifierTemplate.Evasion):
                        modifierTemplate.Evasion = update.Value.SafeParseInt();
                        break;
                }
            }
        }
    }
}
