namespace brokenHeart.Models.DataTransfer.Projection
{
    public class ElementView
    {
        public List<Text> Texts { get; set; }
        public List<Field> Fields { get; set; }
        public List<Relation> Relations { get; set; }

        public class Text
        {
            public string? FieldId { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
        }

        public class Field
        {
            public string? FieldId { get; set; }
            public string Title { get; set; }
            public FieldType Type { get; set; }
            public dynamic? Content { get; set; }

            public class MultiField
            {
                public string Separator { get; set; }
                public List<Field> Fields { get; set; }
            }

            public class EnumContent
            {
                public EnumType Type { get; set; }
                public int Value { get; set; }

                public enum EnumType
                {
                    TargetType,
                    ReplenishType
                }
            }
        }

        public enum FieldType
        {
            String,
            Number,
            Boolean,
            Multi,
            Enum,
            Fixed
        }

        public class Relation
        {
            public string Title { get; set; }
            public RelationType RelationType { get; set; }
            public dynamic? RelationItems { get; set; }
            public ElementType? ElementType { get; set; }

            public class ElementRelationItem
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }

            public class RollRelationItem
            {
                public int Id { get; set; }
                public string Name { get; set; }
                public string Roll { get; set; }
            }

            public class StatRelationItem
            {
                public int Id { get; set; }
                public int StatId { get; set; }
                public string Name { get; set; }
                public int Value { get; set; }
            }
        }

        public enum RelationType
        {
            SingleElement,
            MultipleElements,
            Stats,
            Roll
        }
    }
}
