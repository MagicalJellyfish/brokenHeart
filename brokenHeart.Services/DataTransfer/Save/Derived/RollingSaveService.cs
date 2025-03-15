using brokenHeart.Database.DAO.Abilities;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.Interfaces;
using brokenHeart.DB;
using brokenHeart.Models.DataTransfer;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Services.DataTransfer.Save.Auxiliary
{
    internal class RollingSaveService : IRollingSaveService
    {
        private readonly BrokenDbContext _context;

        public RollingSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public void UpdateRolls<T>(int id, List<RollModel> rolls)
            where T : class
        {
            IQueryable<IRolling> rollables = _context.Set<T>() as IQueryable<IRolling>;
            IRolling rollable = rollables.Include(x => x.Rolls).Single(x => x.Id == id);

            foreach (RollModel roll in rolls)
            {
                Roll? rollableRoll = rollable.Rolls.SingleOrDefault(x => x.Id == roll.Id);
                if (rollableRoll == null)
                {
                    rollableRoll = new Roll();
                    _context.Rolls.Add(rollableRoll);
                }

                if (typeof(Ability).IsAssignableFrom(typeof(T)))
                {
                    rollableRoll.AbilityId = id;
                }
                if (typeof(AbilityTemplate).IsAssignableFrom(typeof(T)))
                {
                    rollableRoll.AbilityTemplateId = id;
                }

                rollableRoll.Name = roll.Name;
                rollableRoll.Instruction = roll.Value;
            }

            List<Roll> rollsToRemove = new List<Roll>();
            foreach (Roll roll in rollable.Rolls)
            {
                if (!rolls.Select(x => x.Id).Contains(roll.Id))
                {
                    rollsToRemove.Add(roll);
                }
            }

            foreach (Roll roll in rollsToRemove)
            {
                _context.Rolls.Remove(roll);
            }

            _context.SaveChanges();
        }
    }
}
