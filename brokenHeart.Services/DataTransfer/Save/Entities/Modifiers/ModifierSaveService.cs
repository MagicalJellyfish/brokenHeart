using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers
{
    internal class ModifierSaveService : IModifierSaveService
    {
        private readonly BrokenDbContext _context;

        public ModifierSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public void UpdateGivenModifier(Modifier modifier, List<ElementUpdate> updates)
        {
            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(Modifier.Name):
                        modifier.Name = update.Value;
                        break;
                    case nameof(Modifier.Abstract):
                        modifier.Abstract = update.Value;
                        break;
                    case nameof(Modifier.Description):
                        modifier.Description = update.Value;
                        break;
                    case nameof(Modifier.MaxHp):
                        modifier.MaxHp = update.Value.SafeParseInt();
                        break;
                    case nameof(Modifier.MovementSpeed):
                        modifier.MovementSpeed = update.Value.SafeParseInt();
                        break;
                    case nameof(Modifier.Armor):
                        modifier.Armor = update.Value.SafeParseInt();
                        break;
                    case nameof(Modifier.Evasion):
                        modifier.Evasion = update.Value.SafeParseInt();
                        break;
                }
            }
        }
    }
}
