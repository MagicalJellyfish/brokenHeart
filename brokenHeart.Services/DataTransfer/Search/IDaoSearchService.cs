using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search
{
    internal interface IDaoSearchService
    {
        public IQueryable<T> GetElements<T>(DaoSearch? search = null)
            where T : class;

        public IQueryable<T> GetSingleElement<T>(DaoSearch search)
            where T : class;
    }
}
