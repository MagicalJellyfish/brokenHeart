using brokenHeart.Services.DataTransfer;
using brokenHeart.Services.DataTransfer.Projection;
using brokenHeart.Services.DataTransfer.Projection.Abilities;
using brokenHeart.Services.DataTransfer.Projection.Characters;
using brokenHeart.Services.DataTransfer.Projection.Counters;
using brokenHeart.Services.DataTransfer.Projection.Modifiers.Effects;
using brokenHeart.Services.DataTransfer.Projection.Modifiers.Items;
using brokenHeart.Services.DataTransfer.Projection.Modifiers.Traits;
using brokenHeart.Services.DataTransfer.Projection.RoundReminders;
using brokenHeart.Services.DataTransfer.Projection.Stats;
using brokenHeart.Services.DataTransfer.Projection.Templates;
using brokenHeart.Services.DataTransfer.Save;
using brokenHeart.Services.DataTransfer.Save.Abilities;
using brokenHeart.Services.DataTransfer.Save.Auxiliary;
using brokenHeart.Services.DataTransfer.Save.Characters;
using brokenHeart.Services.DataTransfer.Save.Counters;
using brokenHeart.Services.DataTransfer.Save.Derived;
using brokenHeart.Services.DataTransfer.Save.Entities.Counters;
using brokenHeart.Services.DataTransfer.Save.Entities.Modifiers;
using brokenHeart.Services.DataTransfer.Save.Entities.Modifiers.Effects;
using brokenHeart.Services.DataTransfer.Save.Entities.Modifiers.Items;
using brokenHeart.Services.DataTransfer.Save.Entities.Modifiers.Traits;
using brokenHeart.Services.DataTransfer.Save.Entities.RoundReminders;
using brokenHeart.Services.DataTransfer.Save.Modifiers;
using brokenHeart.Services.DataTransfer.Save.Modifiers.Effects;
using brokenHeart.Services.DataTransfer.Save.Modifiers.Items;
using brokenHeart.Services.DataTransfer.Save.Modifiers.Traits;
using brokenHeart.Services.DataTransfer.Save.RoundReminders;
using brokenHeart.Services.DataTransfer.Search;
using brokenHeart.Services.Endpoints;
using brokenHeart.Services.Rolling;
using brokenHeart.Services.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace brokenHeart.Services
{
    public static class ServiceRegistration
    {
        // csharpier-ignore
        public static void RegisterServices(IServiceCollection serviceCollection)
        {
            // Various
            serviceCollection.AddScoped<IEndpointEntityService, EndpointEntityService>();
            serviceCollection.AddScoped<IRollService, RollService>();

            serviceCollection.AddScoped<IElementDeterminationService, ElementDeterminationService>();

            // Save
            serviceCollection.AddScoped<ICharacterSaveService, CharacterSaveService>();
            serviceCollection.AddScoped<IModifierSaveService, ModifierSaveService>();
            serviceCollection.AddScoped<IModifierTemplateSaveService, ModifierTemplateSaveService>();

            serviceCollection.AddScoped<IRollingSaveService, RollingSaveService>();
            serviceCollection.AddScoped<IOrderableSaveService, OrderableSaveService>();
            serviceCollection.AddScoped<IStatValueElementSaveService, StatValueElementSaveService>();

            // Projections
            serviceCollection.AddScoped<ICharacterProjectionService, CharacterProjectionService>();
            serviceCollection.AddScoped<IStatProjectionService, StatProjectionService>();
            serviceCollection.AddScoped<ITemplateListProjectionService, TemplateListProjectionService>();

            // Search
            serviceCollection.AddScoped<IDaoSearchService, DaoSearchService>();
            serviceCollection.AddScoped<ICharacterSearchService, CharacterSearchService>();

            // Element Retrieval
            serviceCollection.AddScoped<IElementRetrievalService, ElementRetrievalService>();

            serviceCollection.AddScoped<IElementProjectionService, AbilityProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, TraitProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, ItemProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, EffectProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, CounterProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, EffectCounterProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, RoundReminderProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, VariableProjectionService>();

            serviceCollection.AddScoped<IElementProjectionService, AbilityTemplateProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, TraitTemplateProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, ItemTemplateProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, EffectTemplateProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, CounterTemplateProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, EffectCounterTemplateProjectionService>();
            serviceCollection.AddScoped<IElementProjectionService, RoundReminderTemplateProjectionService>();

            serviceCollection.AddScoped<ITemplateProjectionService, AbilityTemplateProjectionService>();
            serviceCollection.AddScoped<ITemplateProjectionService, TraitTemplateProjectionService>();
            serviceCollection.AddScoped<ITemplateProjectionService, ItemTemplateProjectionService>();
            serviceCollection.AddScoped<ITemplateProjectionService, EffectTemplateProjectionService>();
            serviceCollection.AddScoped<ITemplateProjectionService, CounterTemplateProjectionService>();
            serviceCollection.AddScoped<ITemplateProjectionService, EffectCounterTemplateProjectionService>();
            serviceCollection.AddScoped<ITemplateProjectionService, RoundReminderTemplateProjectionService>();

            // Element Submission
            serviceCollection.AddScoped<IElementSubmissionService, ElementSubmissionService>();

            serviceCollection.AddScoped<IElementSaveService, AbilitySaveService>();
            serviceCollection.AddScoped<IElementSaveService, TraitSaveService>();
            serviceCollection.AddScoped<IElementSaveService, ItemSaveService>();
            serviceCollection.AddScoped<IElementSaveService, EffectSaveService>();
            serviceCollection.AddScoped<IElementSaveService, CounterSaveService>();
            serviceCollection.AddScoped<IElementSaveService, EffectCounterSaveService>();
            serviceCollection.AddScoped<IElementSaveService, RoundReminderSaveService>();
            serviceCollection.AddScoped<IElementSaveService, VariableSaveService>();

            serviceCollection.AddScoped<IElementSaveService, AbilityTemplateSaveService>();
            serviceCollection.AddScoped<IElementSaveService, TraitTemplateSaveService>();
            serviceCollection.AddScoped<IElementSaveService, ItemTemplateSaveService>();
            serviceCollection.AddScoped<IElementSaveService, EffectTemplateSaveService>();
            serviceCollection.AddScoped<IElementSaveService, CounterTemplateSaveService>();
            serviceCollection.AddScoped<IElementSaveService, EffectCounterTemplateSaveService>();
            serviceCollection.AddScoped<IElementSaveService, RoundReminderTemplateSaveService>();

            serviceCollection.AddScoped<ITemplateSaveService, AbilityTemplateSaveService>();
            serviceCollection.AddScoped<ITemplateSaveService, TraitTemplateSaveService>();
            serviceCollection.AddScoped<ITemplateSaveService, ItemTemplateSaveService>();
            serviceCollection.AddScoped<ITemplateSaveService, EffectTemplateSaveService>();
            serviceCollection.AddScoped<ITemplateSaveService, CounterTemplateSaveService>();
            serviceCollection.AddScoped<ITemplateSaveService, EffectCounterTemplateSaveService>();
            serviceCollection.AddScoped<ITemplateSaveService, RoundReminderTemplateSaveService>();

            // SignalR
            serviceCollection.AddHostedService<SignalRMessagingService>();
        }
    }
}
