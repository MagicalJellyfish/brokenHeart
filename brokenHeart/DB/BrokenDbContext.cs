using brokenHeart.Entities.Characters;
using brokenHeart.Entities.Counters;
using brokenHeart.Entities.Items;
using brokenHeart.Entities.Effects;
using brokenHeart.Entities.Traits;
using Microsoft.EntityFrameworkCore;
using brokenHeart.Entities;
using brokenHeart.Controllers;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using brokenHeart.Entities.RoundReminders;
using brokenHeart.Entities.Stats;
using brokenHeart.Entities.Combat;
using brokenHeart.Entities.Effects.Injuries;
using brokenHeart.Entities.Abilities.Abilities;
using brokenHeart.Entities.Abilities;

namespace brokenHeart.DB
{
    public class BrokenDbContext : DbContext
    {
        public DbSet<UserSimplified> UserSimplified { get; set; }

        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterTemplate> CharacterTemplates { get; set; }
        public DbSet<Bodypart> Bodyparts { get; set; }
        public DbSet<BodypartCondition> BodypartConditions { get; set; }

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

        public BrokenDbContext(DbContextOptions<BrokenDbContext> options, CharChangeObservable observable) : base(options)
        {
            ccObservable = observable;
        }

        private CharChangeObservable ccObservable;

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
            foreach (var entity in ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged))
            {
                changedEntries.Add(entity);
            }

            foreach(var entry in changedEntries)
            {
                bool deleted = false;
                if(entry.State == EntityState.Deleted)
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
                        Character c = Characters
                            .Include(x => x.Items).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)
                            .Include(x => x.Traits).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)
                            .Include(x => x.Effects).ThenInclude(x => x.StatIncreases).ThenInclude(x => x.Stat)
                            .Single(x => x.Id == changedChar);

                        c.Update();
                        saveChanges += base.SaveChanges();

                        ccObservable.Trigger(changedChar);
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

            if(entry.Entity.GetType().BaseType == Stats.GetType().GetGenericArguments()[0] || entry.Entity.GetType().BaseType == Bodyparts.GetType().GetGenericArguments()[0])
            {
                return -1;
            }

            foreach (var navigationEntry in entry.Navigations)
            {
                if(navigationEntry is CollectionEntry collectionEntry)
                {
                    collectionEntry.Load();
                    foreach(var referencedEntity in collectionEntry.CurrentValue)
                    {
                        dynamic referencedEntry = Entry(referencedEntity);

                        if (!checkedEntries.Any(x => x.Entity.GetType() == referencedEntry.Entity.GetType() && x.Entity.Id == referencedEntry.Entity.Id))
                        {
                            checkedEntries.Add(referencedEntry);

                            int result = RecursiveSearch(referencedEntry);
                            if(result != -1)
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

                    if(referencedEntry != null)
                    {
                        if (!checkedEntries.Any(x => x.Entity.GetType() == referencedEntry.Entity.GetType() && x.Entity.Id == referencedEntry.Entity.Id))
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
                x.HasOne("brokenHeart.Entities.Character", "Character")
                    .WithMany("Stats")
                    .HasForeignKey("CharacterId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Entities.Modifier", "Modifier")
                    .WithMany("StatIncreases")
                    .HasForeignKey("ModifierId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Entities.ModifierTemplate", "ModifierTemplate")
                    .WithMany("StatIncreases")
                    .HasForeignKey("ModifierTemplateId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Counter>(x =>
            {
                x.HasOne("brokenHeart.Entities.Character", "Character")
                    .WithMany("Counters")
                    .HasForeignKey("CharacterId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Entities.Modifier", "Modifier")
                    .WithMany("Counters")
                    .HasForeignKey("ModifierId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RoundReminder>(x =>
            {
                x.HasOne("brokenHeart.Entities.Character", "Character")
                    .WithMany("RoundReminders")
                    .HasForeignKey("CharacterId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Entities.Counters.Counter", "Counter")
                       .WithOne("RoundReminder")
                       .HasForeignKey("brokenHeart.Entities.RoundReminders.RoundReminder", "CounterId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Entities.Modifier", "Modifier")
                    .WithOne("RoundReminder")
                    .HasForeignKey("brokenHeart.Entities.RoundReminders.RoundReminder", "ModifierId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CombatEntry>(x =>
            {
                x.HasOne("brokenHeart.Entities.Character", "Character")
                    .WithMany()
                    .HasForeignKey("CharacterId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Entities.Combat.Combat", null)
                    .WithMany("Entries")
                    .HasForeignKey("CombatId")
                    .OnDelete(DeleteBehavior.Cascade);

                x.HasOne("brokenHeart.Entities.Combat.Event", "Event")
                    .WithOne("CombatEntry")
                    .HasForeignKey("brokenHeart.Entities.Combat.CombatEntry", "EventId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InjuryEffect>(x =>
            {
                x.HasOne("brokenHeart.Entities.Character", "CharacterInjury")
                    .WithMany("InjuryEffects")
                    .HasForeignKey("CharacterInjuryId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Character>().
                HasOne("brokenHeart.Entities.UserSimplified", "Owner")
                .WithMany("Characters")
                .HasForeignKey("OwnerId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Effect>()
                .HasOne("brokenHeart.Entities.Character", "Character")
                .WithMany("Effects")
                .HasForeignKey("CharacterId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
