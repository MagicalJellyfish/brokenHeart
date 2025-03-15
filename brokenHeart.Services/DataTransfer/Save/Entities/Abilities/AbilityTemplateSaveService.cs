using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.DataTransfer.Save.Entities;
using brokenHeart.Services.Utility;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.DataTransfer.Save.Abilities
{
    internal class AbilityTemplateSaveService : IElementSaveService, ITemplateSaveService
    {
        public ElementType SaveType => ElementType.AbilityTemplate;

        private readonly BrokenDbContext _context;
        private readonly IOrderableSaveService _orderableSaveService;

        public AbilityTemplateSaveService(
            BrokenDbContext context,
            IOrderableSaveService orderableSaveService
        )
        {
            _context = context;
            _orderableSaveService = orderableSaveService;
        }

        public int CreateElement(ElementParentType parentType, int? parentId)
        {
            AbilityTemplate abilityTemplate = new AbilityTemplate();

            if (parentType != ElementParentType.None)
            {
                Assign(abilityTemplate, parentType, (int)parentId);
            }

            _context.AbilityTemplates.Add(abilityTemplate);
            _context.SaveChanges();

            return abilityTemplate.Id;
        }

        public void ReorderElements(List<ElementReorder> reorders)
        {
            _orderableSaveService.ReorderElements<AbilityTemplate>(reorders);
        }

        public void UpdateElement(int id, List<ElementUpdate> updates)
        {
            AbilityTemplate abilityTemplate = _context.AbilityTemplates.Single(x => x.Id == id);

            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(AbilityTemplate.Name):
                        abilityTemplate.Name = update.Value;
                        break;
                    case nameof(AbilityTemplate.Abstract):
                        abilityTemplate.Abstract = update.Value;
                        break;
                    case nameof(AbilityTemplate.Description):
                        abilityTemplate.Description = update.Value;
                        break;
                    case nameof(AbilityTemplate.Shortcut):
                        abilityTemplate.Shortcut = update.Value;
                        break;
                    case nameof(AbilityTemplate.TargetType):
                        abilityTemplate.TargetType = Enum.Parse<TargetType>(update.Value);
                        break;
                    case nameof(AbilityTemplate.CanInjure):
                        abilityTemplate.CanInjure = bool.Parse(update.Value);
                        break;
                    case nameof(AbilityTemplate.Self):
                        abilityTemplate.Self = update.Value;
                        break;
                    case nameof(AbilityTemplate.Target):
                        abilityTemplate.Target = update.Value;
                        break;
                    case nameof(AbilityTemplate.Damage):
                        abilityTemplate.Damage = update.Value;
                        break;
                    case nameof(AbilityTemplate.Range):
                        abilityTemplate.Range = update.Value;
                        break;
                    case nameof(AbilityTemplate.MaxUses):
                        abilityTemplate.MaxUses = update.Value.SafeParseInt();
                        break;
                    case nameof(AbilityTemplate.ReplenishType):
                        abilityTemplate.ReplenishType = Enum.Parse<ReplenishType>(update.Value);
                        break;
                }
            }

            _context.SaveChanges();
        }

        public void DeleteElement(int id)
        {
            _context.AbilityTemplates.Remove(_context.AbilityTemplates.Single(x => x.Id == id));
            _context.SaveChanges();
        }

        public void RelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            AbilityTemplate abilityTemplate = _context.AbilityTemplates.Single(x => x.Id == id);
            Assign(abilityTemplate, parentType, parentId);

            _context.SaveChanges();
        }

        public void UnrelateTemplate(int id, ElementParentType parentType, int parentId)
        {
            AbilityTemplate abilityTemplate = _context
                .AbilityTemplates.Include(x => x.CharacterTemplates)
                .Include(x => x.ModifierTemplates)
                .Single(x => x.Id == id);

            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    abilityTemplate.CharacterTemplates.Remove(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                case ElementParentType.ModifierTemplate:
                    abilityTemplate.ModifierTemplates.Remove(
                        _context.ModifierTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }

            _context.SaveChanges();
        }

        public int InstantiateTemplate(int id, ElementParentType parentType, int parentId)
        {
            IQueryable<AbilityTemplate> abilityTemplate = _context.AbilityTemplates.Where(x =>
                x.Id == id
            );

            Ability ability = abilityTemplate
                .Select(x => Instantiation.InstantiateAbility.Invoke(x))
                .Single();

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

        private void Assign(
            AbilityTemplate abilityTemplate,
            ElementParentType parentType,
            int parentId
        )
        {
            switch (parentType)
            {
                case ElementParentType.CharacterTemplate:
                    abilityTemplate.CharacterTemplates.Add(
                        _context.CharacterTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                case ElementParentType.ModifierTemplate:
                    abilityTemplate.ModifierTemplates.Add(
                        _context.ModifierTemplates.Single(x => x.Id == parentId)
                    );
                    break;
                default:
                    throw new Exception($"Parent type {parentType.ToString()} is invalid");
            }
        }
    }
}
