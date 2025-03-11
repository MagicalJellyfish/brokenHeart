using brokenHeart.Database.DAO.Abilities;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.Utility;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.DataTransfer.Save.Abilities
{
    internal class AbilitySaveService : IElementSaveService, IAbilitySaveService
    {
        public ElementType SaveType => ElementType.Ability;

        private readonly BrokenDbContext _context;

        public AbilitySaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public void UpdateRolls(int id, List<RollModel> rolls)
        {
            Ability ability = _context.Abilities.Include(x => x.Rolls).Single(x => x.Id == id);

            foreach (RollModel roll in rolls)
            {
                Roll? abilityRoll = ability.Rolls.SingleOrDefault(x => x.Id == roll.Id);
                if (abilityRoll == null)
                {
                    abilityRoll = new Roll() { AbilityId = ability.Id };
                    _context.Rolls.Add(abilityRoll);
                }

                abilityRoll.Name = roll.Name;
                abilityRoll.Instruction = roll.Value;
            }

            List<Roll> rollsToRemove = new List<Roll>();
            foreach (Roll roll in ability.Rolls)
            {
                if (!rolls.Select(x => x.Id).Contains(roll.Id))
                {
                    rollsToRemove.Add(roll);
                }
            }

            foreach (Roll roll in rollsToRemove)
            {
                _context.Rolls.Remove(roll);
            }

            _context.SaveChanges();
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
