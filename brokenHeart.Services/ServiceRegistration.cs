using brokenHeart.Services.DataTransfer;
using brokenHeart.Services.DataTransfer.Projection;
using brokenHeart.Services.DataTransfer.Projection.Abilities;
using brokenHeart.Services.DataTransfer.Projection.Characters;
using brokenHeart.Services.DataTransfer.Projection.RoundReminders;
using brokenHeart.Services.DataTransfer.Projection.Stats;
using brokenHeart.Services.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Abilities;
using brokenHeart.Services.DataTransfer.Save.Characters;
using brokenHeart.Services.DataTransfer.Save.Counters;
using brokenHeart.Services.DataTransfer.Save.Modifiers;
using brokenHeart.Services.DataTransfer.Save.Modifiers.Effects;
using brokenHeart.Services.DataTransfer.Save.Modifiers.Items;
using brokenHeart.Services.DataTransfer.Save.Modifiers.Traits;
using brokenHeart.Services.DataTransfer.Save.RoundReminders;
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
            serviceCollection.AddScoped<IModifierSaveService, ModifierSaveService>();
            serviceCollection.AddScoped<IAbilitySaveService, AbilitySaveService>();

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

            // Element Submission
            serviceCollection.AddScoped<IElementSubmissionService, ElementSubmissionService>();

            serviceCollection.AddScoped<IElementSaveService, AbilitySaveService>();
            serviceCollection.AddScoped<IElementSaveService, TraitSaveService>();
            serviceCollection.AddScoped<IElementSaveService, ItemSaveService>();
            serviceCollection.AddScoped<IElementSaveService, EffectSaveService>();
            serviceCollection.AddScoped<IElementSaveService, CounterSaveService>();
            serviceCollection.AddScoped<IElementSaveService, RoundReminderSaveService>();
            serviceCollection.AddScoped<IElementSaveService, VariableSaveService>();

            // SignalR
            serviceCollection.AddHostedService<SignalRMessagingService>();
        }
    }
}
