using brokenHeart.Database.DAO;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Stats;
using brokenHeart.DB;
using brokenHeart.Models;
using brokenHeart.Models.DataTransfer.Save;
using brokenHeart.Services.Utility;

namespace brokenHeart.Services.DataTransfer.Save.Characters
{
    internal class CharacterSaveService : ICharacterSaveService
    {
        private readonly BrokenDbContext _context;

        public CharacterSaveService(BrokenDbContext context)
        {
            _context = context;
        }

        public ExecutionResult<int> CreateCharacter(string username)
        {
            UserSimplified? requester = _context.UserSimplified.SingleOrDefault(x =>
                x.Username.ToLower() == username.ToLower()
            );

            if (requester == null)
            {
                return new ExecutionResult<int>()
                {
                    Succeeded = false,
                    Message = $"User with username {username} could not be found.",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }

            Character c = new Character();

            c.Owner = requester;

            c.DeathCounter = new Counter(
                "Dying",
                3,
                "This counter indicates the number of rounds you are away from dying.",
                false
            );

            foreach (Bodypart bodypart in _context.Bodyparts)
            {
                c.BodypartConditions.Add(
                    new BodypartCondition() { Bodypart = bodypart, InjuryLevel = InjuryLevel.None }
                );
            }

            foreach (Stat stat in _context.Stats)
            {
                c.Stats.Add(new StatValue() { Stat = stat, Value = 0 });
            }

            _context.Characters.Add(c);
            _context.SaveChanges();

            return new ExecutionResult<int>() { Value = c.Id };
        }

        public ExecutionResult PatchCharacter(int id, List<CharacterPatch> patches)
        {
            Character? c = _context.Characters.SingleOrDefault(x => x.Id == id);

            if (c == null)
            {
                return new ExecutionResult()
                {
                    Succeeded = false,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Message = $"No character found for id {id}."
                };
            }

            foreach (CharacterPatch patch in patches)
            {
                switch (patch.TargetProperty)
                {
                    case CharacterPatch.Property.Name:
                        c.Name = patch.Value;
                        break;
                    case CharacterPatch.Property.Experience:
                        c.Experience = patch.Value;
                        break;
                    case CharacterPatch.Property.Notes:
                        c.Notes = patch.Value;
                        break;
                    case CharacterPatch.Property.Description:
                        c.Description = patch.Value;
                        break;
                    case CharacterPatch.Property.Hp:
                        c.Hp = patch.Value.SafeParseInt();
                        break;
                    case CharacterPatch.Property.TempHp:
                        c.TempHp = patch.Value.SafeParseInt();
                        break;
                    case CharacterPatch.Property.Age:
                        c.Age = patch.Value.SafeParseInt();
                        break;
                    case CharacterPatch.Property.Height:
                        c.Height = patch.Value.SafeParseDecimal();
                        break;
                    case CharacterPatch.Property.Weight:
                        c.Weight = patch.Value.SafeParseInt();
                        break;
                    case CharacterPatch.Property.IsNPC:
                        c.IsNPC = bool.Parse(patch.Value);
                        break;
                    case CharacterPatch.Property.DefaultShortcut:
                        c.DefaultShortcut = patch.Value;
                        break;
                    case CharacterPatch.Property.Money:
                        c.Money = patch.Value.SafeParseDecimal();
                        break;
                    case CharacterPatch.Property.C:
                        c.C = patch.Value.SafeParseInt();
                        break;
                }
            }

            _context.SaveChanges();

            return new ExecutionResult();
        }

        public void DeleteCharacter(int id)
        {
            _context.Characters.Remove(_context.Characters.Single(x => x.Id == id));
            _context.SaveChanges();
        }
    }
}
