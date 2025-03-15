namespace brokenHeart.Models.DataTransfer.Projection
{
    public class TemplateListModels
    {
        public class AbstractTemplateModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Abstract { get; set; }
        }

        public class CounterTemplateModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class RoundReminderTemplateModel
        {
            public int Id { get; set; }
            public string Reminder { get; set; }
        }

        public static List<ElementList.ElementColumn> AbstractTemplateColumns =
            new List<ElementList.ElementColumn>()
            {
                new ElementList.ElementColumn()
                {
                    Title = "ID",
                    Property = "id",
                    ColumnType = ElementList.ElementColumnType.Text,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Name",
                    Property = "name",
                    ColumnType = ElementList.ElementColumnType.Text,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Abstract",
                    Property = "abstract",
                    ColumnType = ElementList.ElementColumnType.Text,
                }
            };

        public static List<ElementList.ElementColumn> CounterTemplateColumns =
            new List<ElementList.ElementColumn>()
            {
                new ElementList.ElementColumn()
                {
                    Title = "ID",
                    Property = "id",
                    ColumnType = ElementList.ElementColumnType.Text,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Name",
                    Property = "name",
                    ColumnType = ElementList.ElementColumnType.Text,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Description",
                    Property = "description",
                    ColumnType = ElementList.ElementColumnType.Text,
                }
            };

        public static List<ElementList.ElementColumn> RoundReminderTemplateColumns =
            new List<ElementList.ElementColumn>()
            {
                new ElementList.ElementColumn()
                {
                    Title = "ID",
                    Property = "id",
                    ColumnType = ElementList.ElementColumnType.Text,
                },
                new ElementList.ElementColumn()
                {
                    Title = "Reminder",
                    Property = "reminder",
                    ColumnType = ElementList.ElementColumnType.Text,
                }
            };
    }
}
