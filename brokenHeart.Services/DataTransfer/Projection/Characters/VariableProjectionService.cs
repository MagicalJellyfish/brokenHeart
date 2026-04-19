using brokenHeart.Database.DAO.Characters;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;

namespace brokenHeart.Services.DataTransfer.Projection.Characters
{
    internal class VariableProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Variable;

        private readonly BrokenDbContext _context;

        public VariableProjectionService(BrokenDbContext context)
        {
            _context = context;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<Variable> variables = _context.Variables.Where(x => x.Id == id);

            return variables
                .Select(x => new ElementView()
                {
                    Texts = new() { },
                    Fields = new()
                    {
                        new ElementView.Field()
                        {
                            Title = "Id",
                            Content = x.Id,
                            Type = ElementView.FieldType.Fixed,
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Variable.Name),
                            Title = "Name",
                            Content = x.Name,
                            Type = ElementView.FieldType.String,
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Variable.Value),
                            Title = "Value",
                            Content = x.Value,
                            Type = ElementView.FieldType.Number,
                        },
                    },
                    Relations = new() { },
                })
                .SingleOrDefault();
        }
    }
}
