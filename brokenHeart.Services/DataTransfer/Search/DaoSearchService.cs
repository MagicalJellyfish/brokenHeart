using brokenHeart.Database.Interfaces;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Search;

namespace brokenHeart.Services.DataTransfer.Search
{
    internal class DaoSearchService : IDaoSearchService
    {
        private readonly BrokenDbContext _context;

        public DaoSearchService(BrokenDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetElements<T>(DaoSearch? search)
            where T : class
        {
            IQueryable<IDao> elements = _context.Set<T>() as IQueryable<IDao>;

            if (search != null)
            {
                if (search.Id != null)
                {
                    elements = elements.Where(x => x.Id == search.Id);
                }
            }

            return elements.Cast<T>();
        }

        public IQueryable<T> GetSingleElement<T>(DaoSearch search)
            where T : class
        {
            IQueryable<T> result = GetElements<T>(search);

            if (result.Count() < 1 || result.Count() > 1)
            {
                throw new Exception("Found too many elements for search parameters!");
            }

            return result;
        }
    }
}
