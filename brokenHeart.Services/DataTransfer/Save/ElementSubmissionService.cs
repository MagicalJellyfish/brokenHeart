using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.DataTransfer.Save.Derived;

namespace brokenHeart.Services.DataTransfer.Save
{
    internal class ElementSubmissionService : IElementSubmissionService
    {
        private readonly IElementDeterminationService _elementDeterminationService;
        private readonly IEnumerable<IElementSaveService> _elementSaveServices;
        private readonly IEnumerable<ITemplateSaveService> _templateSaveServices;
        private readonly IRollingSaveService _rollingSaveService;
        private readonly IStatValueElementSaveService _statValueElementSaveService;

        public ElementSubmissionService(
            IElementDeterminationService elementDeterminationService,
            IEnumerable<IElementSaveService> elementSaveServices,
            IEnumerable<ITemplateSaveService> templateSaveServices,
            IRollingSaveService rollingSaveService,
            IStatValueElementSaveService statValueElementSaveService
        )
        {
            _elementDeterminationService = elementDeterminationService;
            _elementSaveServices = elementSaveServices;
            _templateSaveServices = templateSaveServices;
            _rollingSaveService = rollingSaveService;
            _statValueElementSaveService = statValueElementSaveService;
        }

        public int CreateElement(ElementType type, ElementType? parentType, int? parentId)
        {
            IElementSaveService elementSaveService = GetElementSaveService(type);
            return elementSaveService.CreateElement(
                _elementDeterminationService.ConvertToParentType(parentType),
                parentId
            );
        }

        public void ReorderElements(ElementType type, List<ElementReorder> reorders)
        {
            IElementSaveService elementSaveService = GetElementSaveService(type);

            elementSaveService.ReorderElements(reorders);
        }

        public void UpdateRolls(ElementType type, int id, List<RollModel> rolls)
        {
            switch (type)
            {
                case ElementType.Ability:
                    _rollingSaveService.UpdateRolls<Ability>(id, rolls);
                    break;
                case ElementType.AbilityTemplate:
                    _rollingSaveService.UpdateRolls<AbilityTemplate>(id, rolls);
                    break;
                default:
                    throw new Exception(
                        "Rolls are only available on Abilities and AbilityTemplates."
                    );
            }
        }

        public void UpdateStats(ElementType type, int id, List<StatValueModel> stats)
        {
            if (_elementDeterminationService.IsTemplate(type))
            {
                _statValueElementSaveService.UpdateStats<ModifierTemplate>(id, stats);
            }
            else
            {
                _statValueElementSaveService.UpdateStats<Modifier>(id, stats);
            }
        }

        public void UpdateElement(ElementType type, int id, List<ElementUpdate> updates)
        {
            IElementSaveService elementSaveService = GetElementSaveService(type);

            updates = updates.Where(x => x.FieldId != null).ToList();
            elementSaveService.UpdateElement(id, updates);
        }

        public void DeleteElement(ElementType type, int id)
        {
            IElementSaveService elementSaveService = GetElementSaveService(type);
            elementSaveService.DeleteElement(id);
        }

        public void RelateTemplate(ElementType type, int id, ElementType parentType, int parentId)
        {
            ITemplateSaveService templateSaveService = GetTemplateSaveService(type);
            templateSaveService.RelateTemplate(
                id,
                _elementDeterminationService.ConvertToParentType(parentType),
                parentId
            );
        }

        public void UnrelateTemplate(ElementType type, int id, ElementType parentType, int parentId)
        {
            ITemplateSaveService templateSaveService = GetTemplateSaveService(type);
            templateSaveService.UnrelateTemplate(
                id,
                _elementDeterminationService.ConvertToParentType(parentType),
                parentId
            );
        }

        private IElementSaveService GetElementSaveService(ElementType type)
        {
            type = _elementDeterminationService.ConvertBaseTypes(type);

            return _elementSaveServices.Single(x => x.SaveType == type);
        }

        private ITemplateSaveService GetTemplateSaveService(ElementType type)
        {
            type = _elementDeterminationService.ConvertBaseTypes(type);

            if (!_elementDeterminationService.IsTemplate(type))
            {
                throw new Exception("Type is not a Template");
            }

            return _templateSaveServices.Single(x => x.SaveType == type);
        }
    }
}
