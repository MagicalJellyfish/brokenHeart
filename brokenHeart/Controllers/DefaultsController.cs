using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultsController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public DefaultsController(BrokenDbContext context)
        {
            _context = context;
        }

        [HttpPatch("character")]
        [Localhost]
        public async Task<ActionResult<string>> DefaultCharacter(ulong discordId, int charId)
        {
            UserSimplified? user = _context
                .UserSimplified.Include(x => x.ActiveCharacter)
                .SingleOrDefault(x => x.DiscordId == discordId);
            if (user == null)
            {
                return NotFound();
            }

            Character character = _context.Characters.SingleOrDefault(x => x.Id == charId);

            if (character == null)
            {
                return NotFound();
            }

            user.ActiveCharacter = character;
            _context.SaveChanges();
            return character.Name;
        }

        [HttpPatch("ability")]
        [Localhost]
        public async Task<ActionResult> DefaultAbility(ulong discordId, string shortcut)
        {
            UserSimplified? user = _context.UserSimplified.SingleOrDefault(x =>
                x.DiscordId == discordId
            );
            if (user == null)
            {
                return NotFound();
            }

            user.DefaultAbilityString = shortcut;
            _context.SaveChanges();
            return Ok();
        }

        [HttpPatch("targets")]
        [Localhost]
        public async Task<ActionResult<string>> DefaultTarget(ulong discordId, string targets)
        {
            UserSimplified? user = _context.UserSimplified.SingleOrDefault(x =>
                x.DiscordId == discordId
            );
            if (user == null)
            {
                return NotFound();
            }

            user.DefaultTargetString = targets;
            _context.SaveChanges();
            return Ok();
        }
    }
}
