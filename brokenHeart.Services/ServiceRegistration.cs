using brokenHeart.Services.Endpoints;
using brokenHeart.Services.Rolling;
using Microsoft.Extensions.DependencyInjection;

namespace brokenHeart.Services
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEndpointEntityService, EndpointEntityService>();
            serviceCollection.AddScoped<IRollService, RollService>();
        }
    }
}
