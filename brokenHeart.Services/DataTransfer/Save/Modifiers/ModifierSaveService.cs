using brokenHeart.Database.DAO.Modifiers;
using brokenHeart.Database.DAO.Stats;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.Utility;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.DataTransfer.Save.Modifiers
{
    internal class ModifierSaveService : IModifierSaveService
    {
        private readonly BrokenDbContext _context;

        public ModifierSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public void UpdateStats(int id, List<StatValueModel> stats)
        {
            Modifier modifier = _context
                .Modifiers.Include(x => x.StatIncreases)
                .Single(x => x.Id == id);

            foreach (StatValueModel stat in stats)
            {
                StatValue? statIncrease = modifier.StatIncreases.SingleOrDefault(x =>
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
                            ModifierId = modifier.Id
                        };
                        _context.StatValues.Add(statIncrease);
                    }

                    statIncrease.Value = stat.Value;
                }
            }

            _context.SaveChanges();
        }

        public void UpdateGivenModifier(Modifier modifier, List<ElementUpdate> updates)
        {
            foreach (ElementUpdate update in updates)
            {
                switch (update.FieldId)
                {
                    case nameof(Modifier.Name):
                        modifier.Name = update.Value;
                        break;
                    case nameof(Modifier.Abstract):
                        modifier.Abstract = update.Value;
                        break;
                    case nameof(Modifier.Description):
                        modifier.Description = update.Value;
                        break;
                    case nameof(Modifier.MaxHp):
                        modifier.MaxHp = update.Value.SafeParseInt();
                        break;
                    case nameof(Modifier.MovementSpeed):
                        modifier.MovementSpeed = update.Value.SafeParseInt();
                        break;
                    case nameof(Modifier.Armor):
                        modifier.Armor = update.Value.SafeParseInt();
                        break;
                    case nameof(Modifier.Evasion):
                        modifier.Evasion = update.Value.SafeParseInt();
                        break;
                }
            }
        }
    }
}
