using brokenHeart.Services.DataTransfer.Projection.Characters;
using brokenHeart.Services.DataTransfer.Search.Characters;
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
            // Various
            serviceCollection.AddScoped<IEndpointEntityService, EndpointEntityService>();
            serviceCollection.AddScoped<IRollService, RollService>();

            // Projections
            serviceCollection.AddScoped<ICharacterProjectionService, CharacterProjectionService>();
            // Search
            serviceCollection.AddScoped<ICharacterSearchService, CharacterSearchService>();
            serviceCollection.AddScoped<ICharacterProjectionService, CharacterProjectionService>();

            // SignalR
            serviceCollection.AddHostedService<SignalRMessagingService>();
        }
    }
}
