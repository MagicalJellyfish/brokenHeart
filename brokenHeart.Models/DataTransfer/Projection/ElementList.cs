namespace brokenHeart.Models.DataTransfer.Projection
{
    public class ElementList
    {
        public string Title { get; set; }
        public bool SubTabs { get; set; } = false;
        public ElementType Type { get; set; }
        public List<ElementColumn> ElementColumns { get; set; }
        public List<dynamic> Elements { get; set; }

        public class ElementColumn
        {
            public string Title { get; set; }
            public int? FieldId { get; set; }
            public string Property { get; set; }
            public string? PropertyOf { get; set; }
            public ElementColumnType ColumnType { get; set; } = ElementColumnType.Text;
        }

        public enum ElementColumnType
        {
            Text,
            Input,
            InputOf,
            Checkbox
        }
    }
}
