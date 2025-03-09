using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Models.DataTransfer.Save.ElementFields.Abilities;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Abilities
{
    internal class AbilitySaveService : IElementSaveService
    {
        public ElementType SaveType => ElementType.Ability;

        private readonly BrokenDbContext _context;

        public AbilitySaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public ExecutionResult<int> CreateElement(ElementParentType parentType, int parentId)
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
                    return new ExecutionResult<int>()
                    {
                        Succeeded = false,
                        Message = $"Parent type {parentType.ToString()} is invalid",
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
            }

            _context.Abilities.Add(ability);
            _context.SaveChanges();

            return new ExecutionResult<int>() { Value = ability.Id };
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            List<Ability> abilities = _context
                .Abilities.Where(x => reorders.Select(y => y.Id).Contains(x.Id))
                .ToList();

            foreach (Ability ability in abilities)
            {
                ability.ViewPosition = reorders.Single(x => x.Id == ability.Id).ViewPosition;
            }

            _context.SaveChanges();
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            Ability ability = _context.Abilities.Single(x => x.Id == id);

            foreach (ElementUpdate update in updates)
            {
                switch ((AbilityField)update.FieldId)
                {
                    case AbilityField.Name:
                        ability.Name = update.Value;
                        break;
                    case AbilityField.Abstract:
                        ability.Abstract = update.Value;
                        break;
                    case AbilityField.Description:
                        ability.Description = update.Value;
                        break;
                    case AbilityField.Shortcut:
                        ability.Shortcut = update.Value;
                        break;
                    case AbilityField.TargetType:
                        ability.TargetType = Enum.Parse<TargetType>(update.Value);
                        break;
                    case AbilityField.CanInjure:
                        ability.CanInjure = bool.Parse(update.Value);
                        break;
                    case AbilityField.Self:
                        ability.Self = update.Value;
                        break;
                    case AbilityField.Target:
                        ability.Target = update.Value;
                        break;
                    case AbilityField.Damage:
                        ability.Damage = update.Value;
                        break;
                    case AbilityField.Range:
                        ability.Range = update.Value;
                        break;
                    case AbilityField.Uses:
                        ability.Uses = update.Value.SafeParseInt();
                        break;
                    case AbilityField.MaxUses:
                        ability.MaxUses = update.Value.SafeParseInt();
                        break;
                    case AbilityField.ReplenishType:
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
