using brokenHeart.Models.DataTransfer;

namespace brokenHeart.Services.DataTransfer
{
    internal class ElementDeterminationService : IElementDeterminationService
    {
        public ElementType ConvertBaseTypes(ElementType type)
        {
            if (type == ElementType.InjuryEffect)
            {
                return ElementType.Effect;
            }
            if (type == ElementType.InjuryEffectTemplate)
            {
                return ElementType.EffectTemplate;
            }

            return type;
        }

        public ElementParentType ConvertToParentType(ElementType? type)
        {
            switch (type)
            {
                case null:
                    return ElementParentType.None;
                case ElementType.Character:
                    return ElementParentType.Character;
                case ElementType.Ability:
                    return ElementParentType.Ability;
                case ElementType.Trait:
                case ElementType.Item:
                case ElementType.Effect:
                    return ElementParentType.Modifier;
                case ElementType.Counter:
                    return ElementParentType.Counter;

                case ElementType.AbilityTemplate:
                    return ElementParentType.AbilityTemplate;
                case ElementType.TraitTemplate:
                case ElementType.ItemTemplate:
                case ElementType.EffectTemplate:
                    return ElementParentType.ModifierTemplate;
                case ElementType.CounterTemplate:
                    return ElementParentType.CounterTemplate;

                default:
                    throw new Exception(
                        "Element Type is not mapped for Element Parent determination"
                    );
            }
        }

        public bool IsTemplate(ElementType type)
        {
            switch (type)
            {
                case ElementType.AbilityTemplate:
                case ElementType.TraitTemplate:
                case ElementType.ItemTemplate:
                case ElementType.EffectTemplate:
                case ElementType.CounterTemplate:
                case ElementType.ReminderTemplate:
                    return true;
                case ElementType.Ability:
                case ElementType.Trait:
                case ElementType.Item:
                case ElementType.Effect:
                case ElementType.Counter:
                case ElementType.Reminder:
                    return false;
                default:
                    throw new Exception("Element Type is not mapped for Template-determination");
            }
        }
    }
}
