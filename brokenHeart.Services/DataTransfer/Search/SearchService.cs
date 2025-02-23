using brokenHeart.DB;

namespace brokenHeart.Services.DataTransfer.Search
{
    public class SearchService
    {
        protected readonly BrokenDbContext _context;

        public SearchService(BrokenDbContext context)
        {
            _context = context;
        }
    }
}
