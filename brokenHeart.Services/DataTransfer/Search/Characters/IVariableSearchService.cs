using brokenHeart.Database.DAO.Characters;
using brokenHeart.Models.DataTransfer.Search.Characters;

namespace brokenHeart.Services.DataTransfer.Search.Characters
{
    internal interface IVariableSearchService
    {
        public IQueryable<Variable> GetVariables(VariableSearch search);
    }
}
