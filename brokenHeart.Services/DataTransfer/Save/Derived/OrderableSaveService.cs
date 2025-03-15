using brokenHeart.Database.Interfaces;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer.Save;

namespace brokenHeart.Services.DataTransfer.Save.Auxiliary
{
    internal class OrderableSaveService : IOrderableSaveService
    {
        private readonly BrokenDbContext _context;

        public OrderableSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public void ReorderElements<T>(List<ElementReorder> reorders)
            where T : class
        {
            IQueryable<IOrderableElement> elements =
                _context.Set<T>() as IQueryable<IOrderableElement>;
            List<IOrderableElement> targetElements = elements
                .Where(x => reorders.Select(y => y.Id).Contains(x.Id))
                .ToList();

            foreach (IOrderableElement element in targetElements)
            {
                element.ViewPosition = reorders.Single(x => x.Id == element.Id).ViewPosition;
            }

            _context.SaveChanges();
        }
    }
}
