using brokenHeart.Services.DataTransfer;
using brokenHeart.Services.DataTransfer.Projection;
using brokenHeart.Services.DataTransfer.Projection.Abilities;
using brokenHeart.Services.DataTransfer.Projection.Characters;
using brokenHeart.Services.DataTransfer.Projection.RoundReminders;
using brokenHeart.Services.DataTransfer.Projection.Stats;
using brokenHeart.Services.DataTransfer.Search.Abilities;
using brokenHeart.Services.DataTransfer.Search.Characters;
using brokenHeart.Services.DataTransfer.Search.RoundReminders;
using brokenHeart.Services.DataTransfer.Search.Stats;
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

            // Save
            serviceCollection.AddScoped<ICharacterSaveService, CharacterSaveService>();

            // Projections
            serviceCollection.AddScoped<ICharacterProjectionService, CharacterProjectionService>();
            serviceCollection.AddScoped<IStatProjectionService, StatProjectionService>();

            // Search
            serviceCollection.AddScoped<ICharacterSearchService, CharacterSearchService>();
            serviceCollection.AddScoped<IAbilitySearchService, AbilitySearchService>();
            serviceCollection.AddScoped<ITraitSearchService, TraitSearchService>();
            serviceCollection.AddScoped<IItemSearchService, ItemSearchService>();
            serviceCollection.AddScoped<IEffectSearchService, EffectSearchService>();
            serviceCollection.AddScoped<ICounterSearchService, CounterSearchService>();
            serviceCollection.AddScoped<IRoundReminderSearchService, RoundReminderSearchService>();
            serviceCollection.AddScoped<IVariableSearchService, VariableSearchService>();
            serviceCollection.AddScoped<IStatSearchService, StatSearchService>();

            // Element Retrieval
            serviceCollection.AddScoped<IElementRetrievalService, ElementRetrievalService>();

            serviceCollection.AddScoped<IElementProjectionService, AbilityProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, TraitProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, ItemProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, EffectProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, CounterProjectionService>();
            serviceCollection.AddScoped<
                IElementProjectionService,
                RoundReminderProjectionService
            >();
            serviceCollection.AddScoped<IElementProjectionService, VariableProjectionService>();
            // SignalR
            serviceCollection.AddHostedService<SignalRMessagingService>();
        }
    }
}
