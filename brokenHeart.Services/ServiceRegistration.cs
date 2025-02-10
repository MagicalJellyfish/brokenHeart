using brokenHeart.Services.Endpoints;
using brokenHeart.Services.Rolling;
using brokenHeart.Services.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace brokenHeart.Services
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEndpointEntityService, EndpointEntityService>();
            serviceCollection.AddScoped<IRollService, RollService>();

            serviceCollection.AddHostedService<SignalRMessagingService>();
        }
    }
}
