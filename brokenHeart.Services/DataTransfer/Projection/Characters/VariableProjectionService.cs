using brokenHeart.Database.DAO.Characters;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Projection;
using brokenHeart.Models.DataTransfer.Save.ElementFields.Characters;
using brokenHeart.Models.DataTransfer.Search.Characters;
using brokenHeart.Services.DataTransfer.Search.Characters;

namespace brokenHeart.Services.DataTransfer.Projection.Characters
{
    internal class VariableProjectionService : IElementProjectionService
    {
        public ElementType ProjectionType => ElementType.Variable;

        private readonly IVariableSearchService _variableSearchService;

        public VariableProjectionService(IVariableSearchService variableSearchService)
        {
            _variableSearchService = variableSearchService;
        }

        public dynamic? GetElement(int id)
        {
            IQueryable<Variable> variables = _variableSearchService.GetVariables(
                new VariableSearch() { Id = id }
            );

            if (variables.Count() == 0 || variables.Count() > 1)
            {
                return null;
            }

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
                            FieldId = (int)VariableField.Name,
                            Title = "Name",
                            Content = x.Name,
                            Type = ElementView.FieldType.String
                        },
                        new ElementView.Field()
                        {
                            FieldId = (int)VariableField.Value,
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
