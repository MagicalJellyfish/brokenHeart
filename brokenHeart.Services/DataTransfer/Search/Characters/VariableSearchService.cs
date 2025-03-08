using brokenHeart.Database.DAO.Characters;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Search.Characters;

namespace brokenHeart.Services.DataTransfer.Search.Characters
{
    internal class VariableSearchService : SearchService, IVariableSearchService
    {
        public VariableSearchService(BrokenDbContext context)
            : base(context) { }

        public IQueryable<Variable> GetVariables(VariableSearch search)
        {
            IQueryable<Variable> variables = _context.Variables;

            if (search.Id != null)
            {
                variables = variables.Where(x => x.Id == search.Id);
            }

            return variables;
        }
    }
}
