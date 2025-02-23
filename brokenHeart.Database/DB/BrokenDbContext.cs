using brokenHeart.Database.DAO;
using brokenHeart.Database.DAO.Abilities;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Combat;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.Database.DAO.Modifiers.Effects.Injuries;
using brokenHeart.Database.DAO.Modifiers.Items;
using brokenHeart.Database.DAO.Modifiers.Traits;
using brokenHeart.Database.DAO.RoundReminders;
using brokenHeart.Database.DAO.Stats;
using brokenHeart.Database.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace brokenHeart.DB
{
    public class BrokenDbContext : DbContext
    {
        public DbSet<UserSimplified> UserSimplified { get; set; }

        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterTemplate> CharacterTemplates { get; set; }
        public DbSet<Bodypart> Bodyparts { get; set; }
        public DbSet<BodypartCondition> BodypartConditions { get; set; }
        public DbSet<Variable> Variables { get; set; }

        public DbSet<RoundReminder> RoundReminders { get; set; }
        public DbSet<RoundReminderTemplate> RoundReminderTemplates { get; set; }

        public DbSet<Counter> Counters { get; set; }
        public DbSet<CounterTemplate> CounterTemplates { get; set; }
        public DbSet<EffectCounter> EffectCounters { get; set; }
        public DbSet<EffectCounterTemplate> EffectCounterTemplates { get; set; }

        public DbSet<Item> Items { get; set; }
        public DbSet<ItemTemplate> ItemTemplates { get; set; }

        public DbSet<Effect> Effects { get; set; }
        public DbSet<EffectTemplate> EffectTemplates { get; set; }

        public DbSet<InjuryEffect> InjuryEffects { get; set; }
        public DbSet<InjuryEffectTemplate> InjuryEffectTemplates { get; set; }

        public DbSet<Trait> Traits { get; set; }
        public DbSet<TraitTemplate> TraitTemplates { get; set; }

        public DbSet<Stat> Stats { get; set; }
        public DbSet<StatValue> StatValues { get; set; }

        public DbSet<Combat> Combats { get; set; }
        public DbSet<CombatEntry> CombatEntries { get; set; }

        public DbSet<Roll> Rolls { get; set; }
        public DbSet<Ability> Abilities { get; set; }
        public DbSet<AbilityTemplate> AbilityTemplates { get; set; }

        public string DbPath { get; }

        public DatabaseEventEmitter _eventEmitter { get; set; }

        public BrokenDbContext(
            DbContextOptions<BrokenDbContext> options,
            DatabaseEventEmitter eventEmitter
        )
            : base(options)
        {
            _eventEmitter = eventEmitter;
        }

        public int SaveChangesSimple()
        {
            return base.SaveChanges();
        }

        public override int SaveChanges()
        {
            return SaveAndNotifyChanges();
        }

        private int SaveAndNotifyChanges()
        {
            int saveChanges = 0;
            List<EntityEntry> changedEntries = new List<EntityEntry>();
            foreach (
                var entity in ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged)
            )
            {
                changedEntries.Add(entity);
            }

            foreach (var entry in changedEntries)
            {
                bool deleted = false;
                if (entry.State == EntityState.Deleted)
                {
                    deleted = true;
                    entry.State = EntityState.Modified;
                }

                int changedChar = RecursiveSearch(entry);

                if (deleted)
                {
                    entry.State = EntityState.Deleted;
                }

                saveChanges += base.SaveChanges();
                if (changedChar != -1)
                {
                    try
                    {
                        // csharpier-ignore
                        Character c = Characters
                            .Include(x => x.Stats)

                            .Include(x => x.Items).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)

                            .Include(x => x.Traits).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)

                            .Include(x => x.Effects).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)

                            .Single(x => x.Id == changedChar);

                        c.Update();
                        saveChanges += base.SaveChanges();

                        _eventEmitter.EmitCharacterChange(changedChar);
                    }
                    catch (Exception)
                    {
                        return -1;
                    }
                }
            }

            return saveChanges;
        }

        private List<dynamic> checkedEntries = new List<dynamic>();

        private int RecursiveSearch(dynamic entry)
        {
            if (entry.Entity.GetType() == Characters.GetType().GetGenericArguments()[0])
            {
                return entry.Entity.Id;
            }

            if (
                entry.Entity.GetType() == Stats.GetType().GetGenericArguments()[0]
                || entry.Entity.GetType() == Bodyparts.GetType().GetGenericArguments()[0]
            )
            {
                return -1;
            }

            foreach (var navigationEntry in entry.Navigations)
            {
                if (navigationEntry is CollectionEntry collectionEntry)
                {
                    collectionEntry.Load();
                    foreach (var referencedEntity in collectionEntry.CurrentValue)
                    {
                        dynamic referencedEntry = Entry(referencedEntity);

                        if (
                            !checkedEntries.Any(x =>
                                x.Entity.GetType() == referencedEntry.Entity.GetType()
                                && x.Entity.Id == referencedEntry.Entity.Id
                            )
                        )
                        {
                            checkedEntries.Add(referencedEntry);

                            int result = RecursiveSearch(referencedEntry);
                            if (result != -1)
                            {
                                return result;
                            }
                        }
                    }
                }
                else if (navigationEntry is ReferenceEntry referenceEntry)
                {
                    referenceEntry.Load();
                    dynamic referencedEntry = referenceEntry.TargetEntry;

                    if (referencedEntry != null)
                    {
                        if (
                            !checkedEntries.Any(x =>
                                x.Entity.GetType() == referencedEntry.Entity.GetType()
                                && x.Entity.Id == referencedEntry.Entity.Id
                            )
                        )
                        {
                            checkedEntries.Add(referencedEntry);

                            int result = RecursiveSearch(referencedEntry);
                            if (result != -1)
                            {
                                return result;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        protected override async void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatValue>(x =>
            {
                x.HasOne("brokenHeart.Database.DAO.Character", "Character")
                    .WithMany("Stats")
                    .HasForeignKey("CharacterId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Database.DAO.Modifiers.Modifier", "Modifier")
                    .WithMany("StatIncreases")
                    .HasForeignKey("ModifierId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Database.DAO.Modifiers.ModifierTemplate", "ModifierTemplate")
                    .WithMany("StatIncreases")
                    .HasForeignKey("ModifierTemplateId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Counter>(x =>
            {
                x.HasOne("brokenHeart.Database.DAO.Character", "Character")
                    .WithMany("Counters")
                    .HasForeignKey("CharacterId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Database.DAO.Modifiers.Modifier", "Modifier")
                    .WithMany("Counters")
                    .HasForeignKey("ModifierId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Variable>(x =>
            {
                x.HasOne("brokenHeart.Database.DAO.Character", "Character")
                    .WithMany("Variables")
                    .HasForeignKey("CharacterId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne(
                        "brokenHeart.Database.DAO.Characters.CharacterTemplate",
                        "CharacterTemplate"
                    )
                    .WithMany("Variables")
                    .HasForeignKey("CharacterTemplateId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RoundReminder>(x =>
            {
                x.HasOne("brokenHeart.Database.DAO.Character", "Character")
                    .WithMany("RoundReminders")
                    .HasForeignKey("CharacterId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Database.DAO.Counters.Counter", "Counter")
                    .WithOne("RoundReminder")
                    .HasForeignKey(
                        "brokenHeart.Database.DAO.RoundReminders.RoundReminder",
                        "CounterId"
                    )
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Database.DAO.Modifiers.Modifier", "Modifier")
                    .WithOne("RoundReminder")
                    .HasForeignKey(
                        "brokenHeart.Database.DAO.RoundReminders.RoundReminder",
                        "ModifierId"
                    )
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CombatEntry>(x =>
            {
                x.HasOne("brokenHeart.Database.DAO.Character", "Character")
                    .WithMany()
                    .HasForeignKey("CharacterId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Database.DAO.Combat.Combat", null)
                    .WithMany("Entries")
                    .HasForeignKey("CombatId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Database.DAO.Combat.Event", "Event")
                    .WithOne("CombatEntry")
                    .HasForeignKey("brokenHeart.Database.DAO.Combat.CombatEntry", "EventId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InjuryEffect>(x =>
            {
                x.HasOne("brokenHeart.Database.DAO.Character", "CharacterInjury")
                    .WithMany("InjuryEffects")
                    .HasForeignKey("CharacterInjuryId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder
                .Entity<Character>()
                .HasOne("brokenHeart.Database.DAO.UserSimplified", "Owner")
                .WithMany("Characters")
                .HasForeignKey("OwnerId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<Effect>()
                .HasOne("brokenHeart.Database.DAO.Character", "Character")
                .WithMany("Effects")
                .HasForeignKey("CharacterId")
                .OnDelete(DeleteBehavior.Cascade);

            //EFCore gets confused on two Many-To-Many Relations on the same table
            modelBuilder
                .Entity<AbilityTemplate>()
                .HasMany(x => x.AppliedEffectTemplates)
                .WithMany(y => y.ApplyingAbilityTemplates);
        }
    }
}
