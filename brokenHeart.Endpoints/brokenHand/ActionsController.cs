using brokenHeart.Database.DAO;
using brokenHeart.DB;
using brokenHeart.Models.brokenHand;
using brokenHeart.Models.Rolling;
using brokenHeart.Services;
using brokenHeart.Services.Logic.AbilityExecution;
using brokenHeart.Services.Rolling;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Localhost]
    public class ActionsController : ControllerBase
    {
        private readonly BrokenDbContext _context;
        private readonly IRollService _rollService;
        private readonly ICharacterRollService _charRollService;
        private readonly IAbilityExecutionService _abilityExecutionService;

        public ActionsController(
            BrokenDbContext brokenDbContext,
            IRollService rollService,
            ICharacterRollService charRollService,
            IAbilityExecutionService abilityExecutionService
        )
        {
            _context = brokenDbContext;
            _rollService = rollService;
            _charRollService = charRollService;
            _abilityExecutionService = abilityExecutionService;
        }

        [HttpGet("rollChar/{id}")]
        public async Task<ActionResult<List<RollResult>>> RollChar(
            int id,
            string rollString,
            int repeat
        )
        {
            return _charRollService.CharRollString(rollString, id, repeat);
        }

        [HttpGet("rollActiveChar/{discordId}")]
        public async Task<ActionResult<List<RollResult>>> RollActiveChar(
            ulong discordId,
            string rollString,
            int repeat
        )
        {
            UserSimplified? user = _context.UserSimplified.SingleOrDefault(x =>
                x.DiscordId == discordId
            );

            if (user == null || user.ActiveCharacterId == null)
            {
                return _rollService.RollString(rollString, repeat: repeat);
            }

            return _charRollService.CharRollString(rollString, (int)user.ActiveCharacterId, repeat);
        }

        [HttpGet("ability")]
        public ActionResult<List<Message>> Ability(
            ulong discordId,
            int? charId,
            string? shortcut,
            string? targets,
            string? selfModifier,
            string? targetModifier,
            string? damageModifier
        )
        {
            UserSimplified? user = _context.UserSimplified.SingleOrDefault(x =>
                x.DiscordId == discordId
            );

            if (user == null)
            {
                return NotFound($"No user found for Discord ID {discordId}!");
            }

            if (shortcut == null)
            {
                if (user.DefaultAbilityString == null)
                {
                    return NotFound(
                        "No shortcut supplied and no default shortcut configured for user!"
                    );
                }

                shortcut = user.DefaultAbilityString;
            }

            if (charId == null)
            {
                if (user.ActiveCharacterId == null)
                {
                    return NotFound(
                        "No character ID supplied and no active character configured for user!"
                    );
                }

                charId = user.ActiveCharacterId;
            }

            if (targets == null)
            {
                // For some abilities no targets are required, so we do not return NotFound as with the other parameters here
                targets = user.DefaultTargetString;
            }

            return _abilityExecutionService.ExecuteAbility(
                (int)charId,
                shortcut,
                targets,
                selfModifier,
                targetModifier,
                damageModifier
            );
        }
    }
}
