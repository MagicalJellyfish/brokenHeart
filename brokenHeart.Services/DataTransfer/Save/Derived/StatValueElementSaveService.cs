using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.DAO.Stats;
using brokenHeart.Database.Interfaces;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.DataTransfer.Save.Derived
{
    internal class StatValueElementSaveService : IStatValueElementSaveService
    {
        private readonly BrokenDbContext _context;

        public StatValueElementSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public void UpdateStats<T>(int id, List<StatValueModel> stats)
            where T : class
        {
            IQueryable<IStatValueElement> statValueElements =
                _context.Set<T>() as IQueryable<IStatValueElement>;
            IStatValueElement statValueElement = statValueElements
                .Include(x => x.StatIncreases)
                .Single(x => x.Id == id);

            foreach (StatValueModel stat in stats)
            {
                StatValue? statIncrease = statValueElement.StatIncreases.SingleOrDefault(x =>
                    x.StatId == stat.Id
                );
                if (stat.Value == 0)
                {
                    if (statIncrease != null)
                    {
                        _context.StatValues.Remove(statIncrease);
                    }
                }
                else
                {
                    if (statIncrease == null)
                    {
                        statIncrease = new StatValue() { StatId = stat.Id, };

                        if (typeof(Modifier).IsAssignableFrom(typeof(T)))
                        {
                            statIncrease.ModifierId = id;
                        }
                        if (typeof(ModifierTemplate).IsAssignableFrom(typeof(T)))
                        {
                            statIncrease.ModifierTemplateId = id;
                        }

                        _context.StatValues.Add(statIncrease);
                    }

                    statIncrease.Value = stat.Value;
                }
            }

            _context.SaveChanges();
        }

        public void UpdateStats(int id, List<StatValueModel> stats)
        {
            ModifierTemplate modifierTemplate = _context
                .ModifierTemplates.Include(x => x.StatIncreases)
                .Single(x => x.Id == id);

            foreach (StatValueModel stat in stats)
            {
                StatValue? statIncrease = modifierTemplate.StatIncreases.SingleOrDefault(x =>
                    x.StatId == stat.Id
                );
                if (stat.Value == 0)
                {
                    if (statIncrease != null)
                    {
                        _context.StatValues.Remove(statIncrease);
                    }
                }
                else
                {
                    if (statIncrease == null)
                    {
                        statIncrease = new StatValue()
                        {
                            StatId = stat.Id,
                            ModifierTemplateId = modifierTemplate.Id
                        };
                        _context.StatValues.Add(statIncrease);
                    }

                    statIncrease.Value = stat.Value;
                }
            }

            _context.SaveChanges();
        }
    }
}
