using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Abilities
{
    internal class AbilitySaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Ability;

        private readonly BrokenDbContext _context;
        private readonly IOrderableSaveService _orderableSaveService;

        public AbilitySaveService(
            BrokenDbContext context,
            IOrderableSaveService orderableSaveService
        )
        {
            _context = context;
            _orderableSaveService = orderableSaveService;
        }

        public int CreateElement(ElementParentType parentType, int? parentId)
        {
            Ability ability = new Ability();

            switch (parentType)
            {
                case ElementParentType.Character:
                    ability.CharacterId = parentId;
                    break;
                case ElementParentType.Modifier:
                    ability.ModifierId = parentId;
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.Abilities.Add(ability);
            _context.SaveChanges();

            return ability.Id;
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<Ability>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            Ability ability = _context.Abilities.Single(x => x.Id == id);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(Ability.Name):
                        ability.Name = update.Value;
                        break;
                    case nameof(Ability.Abstract):
                        ability.Abstract = update.Value;
                        break;
                    case nameof(Ability.Description):
                        ability.Description = update.Value;
                        break;
                    case nameof(Ability.Shortcut):
                        ability.Shortcut = update.Value;
                        break;
                    case nameof(Ability.TargetType):
                        ability.TargetType = Enum.Parse<TargetType>(update.Value);
                        break;
                    case nameof(Ability.CanInjure):
                        ability.CanInjure = bool.Parse(update.Value);
                        break;
                    case nameof(Ability.Self):
                        ability.Self = update.Value;
                        break;
                    case nameof(Ability.Target):
                        ability.Target = update.Value;
                        break;
                    case nameof(Ability.Damage):
                        ability.Damage = update.Value;
                        break;
                    case nameof(Ability.Range):
                        ability.Range = update.Value;
                        break;
                    case nameof(Ability.Uses):
                        ability.Uses = update.Value.SafeParseInt();
                        break;
                    case nameof(Ability.MaxUses):
                        ability.MaxUses = update.Value.SafeParseInt();
                        break;
                    case nameof(Ability.ReplenishType):
                        ability.ReplenishType = Enum.Parse<ReplenishType>(update.Value);
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.Abilities.Remove(_context.Abilities.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
