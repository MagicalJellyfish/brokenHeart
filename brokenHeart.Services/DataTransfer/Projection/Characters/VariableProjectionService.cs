using brokenHeart.Database.DAO.Characters;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Search;
using brokenHeart.Services.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Projection.Characters
{
    internal class VariableProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Variable;

        private readonly IDaoSearchService _daoSearchService;

        public VariableProjectionService(IDaoSearchService daoSearchService)
        {
            _daoSearchService = daoSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<Variable> variables = _daoSearchService.GetSingleElement<Variable>(
                new DaoSearch() { Id = id }
            );

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
                            Type = ElementView.FieldType.Fixed
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Variable.Name),
                            Title = "Name",
                            Content = x.Name,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = nameof(Variable.Value),
                            Title = "Value",
                            Content = x.Value,
                            Type = ElementView.FieldType.Number
                        },
                    },
                    Relations = new() { }
                })
                .SingleOrDefault();
        }
    }
}
