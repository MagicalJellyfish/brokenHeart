using brokenHeart.Database.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace brokenHeart.Database
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<DatabaseEventEmitter>();
        }
    }
}
