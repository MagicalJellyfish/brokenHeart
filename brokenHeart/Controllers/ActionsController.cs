using brokenHeart.Auxiliary;
using brokenHeart.DB;
using brokenHeart.Entities;
using brokenHeart.Entities.Abilities;
using brokenHeart.Entities.Abilities.Abilities;
using brokenHeart.Entities.Combat;
using brokenHeart.Entities.Effects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Localhost]
    public class ActionsController : ControllerBase
    {
        private readonly BrokenDbContext _context;

        public ActionsController(BrokenDbContext brokenDbContext)
        {
            _context = brokenDbContext;
        }

        [HttpGet("roll")]
        public async Task<ActionResult<RollResult>> Roll(string rollString)
        {
            return RollAuxiliary.RollString(rollString);
        }

        [HttpGet("rollChar/{id}")]
        public async Task<ActionResult<RollResult>> RollChar(int id, string rollString)
        {
            Character? c = GetBaseCharacters().SingleOrDefault(x => x.Id == id);

            if(c == null)
            {
                return NotFound("No character found!");
            }

            return RollAuxiliary.CharRollString(rollString, c);
        }

        [HttpGet("rollActiveChar/{discordId}")]
        public async Task<ActionResult<RollResult>> RollActiveChar(ulong discordId, string rollString)
        {
            UserSimplified? user = _context.UserSimplified.Include(x => x.ActiveCharacter).SingleOrDefault(x => x.DiscordId == discordId);

            if (user == null)
            {
                return NotFound("No user found!");
            }

            if (user.ActiveCharacter == null)
            {
                return NotFound("No active character!");
            }

            return RollAuxiliary.CharRollString(rollString, GetBaseCharacters().Single(x => x.Id == user.ActiveCharacter.Id));
        }

        [HttpGet("activeAbility/{discordId}/{shortcut}")]
        public ActionResult<List<Message>> ActiveAbility(ulong discordId, string shortcut, string? targets, bool targetShortcuts = true)
        {
            UserSimplified? user = _context.UserSimplified.Include(x => x.ActiveCharacter).SingleOrDefault(x => x.DiscordId == discordId);

            if (user == null)
            {
                return NotFound("No user found!");
            }

            if (user.ActiveCharacter == null)
            {
                return NotFound("No active character!");
            }

            Character c = GetBaseCharacters()
                .Include(x => x.Abilities).ThenInclude(x => x.Rolls)
                .Include(x => x.Abilities).ThenInclude(x => x.EffectTemplates)!.ThenInclude(x => x.CounterTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.Abilities).ThenInclude(x => x.EffectTemplates)!.ThenInclude(x => x.RoundReminderTemplate)
                .Single(x => x.Id == user.ActiveCharacter.Id);

            return ExecuteAbility(c, shortcut, targets, targetShortcuts);
        }

        [HttpGet("ability/{charId}/{shortcut}")]
        public ActionResult<List<Message>> Ability(int charId, string shortcut, string? targets, bool targetShortcuts = true)
        {
            Character? c = GetBaseCharacters()
                .Include(x => x.Abilities).ThenInclude(x => x.Rolls)
                .Include(x => x.Abilities).ThenInclude(x => x.EffectTemplates)!.ThenInclude(x => x.CounterTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.Abilities).ThenInclude(x => x.EffectTemplates)!.ThenInclude(x => x.RoundReminderTemplate)

                .SingleOrDefault(x => x.Id == charId);

            if (c == null)
            {
                return NotFound("No character found!");
            }

            return ExecuteAbility(c, shortcut, targets, targetShortcuts);
        }

        private ActionResult<List<Message>> ExecuteAbility(Character c, string shortcut, string? targets, bool targetShortcuts)
        {
            Ability? ability;
            try
            {
                ability = c.Abilities.SingleOrDefault(x => x.Shortcut == shortcut);
                if (ability == null)
                {
                    return NotFound($"No ability with shortcut \"{shortcut}\" found!");
                }
            }
            catch
            {
                string conflictList = "";
                foreach (var conflictingAbility in c.Abilities.Where(x => x.Shortcut == shortcut))
                {
                    conflictList += $"{conflictingAbility.Name}, ";
                }

                return BadRequest($"Shortcuts of abilities {conflictList[..1]} are conflicting!");
            }

            List<Character> targetChars = new List<Character>();
            if(!targets.IsNullOrEmpty())
            {
                if (targetShortcuts)
                {
                    Combat? activeCombat = _context.Combats.Include(x => x.Entries).ThenInclude(x => x.Character).SingleOrDefault(x => x.Active);
                    if (activeCombat == null)
                    {
                        return NotFound("No active combat found!");
                    }

                    List<Character> potentialTargets = new List<Character>();
                    potentialTargets = activeCombat.Entries.Where(x => x.Character != null).Select(x => x.Character).ToList()!;
                    if (potentialTargets.Count() == 0)
                    {
                        return NotFound("No potential targets in combat!");
                    }

                    List<Character> expandedTargets = GetBaseCharacters().Where(x => potentialTargets.Contains(x)).ToList();
                    List<string> targetList = targets!.Split(' ').ToList();
                    foreach (string target in targetList)
                    {
                        string fixedTarget = target.ToLower().Trim();
                        CombatEntry? entry = activeCombat.Entries.SingleOrDefault(entry => entry.Shortcut.ToLower().Trim() == fixedTarget && entry.Character != null);
                        if (entry == null)
                        {
                            return NotFound($"No combatant found for shortcut \"{fixedTarget}\"");
                        }

                        Character targetChar = expandedTargets.Single(x => x.Id == entry.Character!.Id);
                        targetChars.Add(targetChar);
                    }
                }
                else
                {
                    List<Character> baseCharacters = GetBaseCharacters().ToList();
                    List<string> targetList = targets!.Split(' ').ToList();
                    foreach (string target in targetList)
                    {
                        int fixedTarget = int.Parse(target.Trim());

                        Character? targetChar = baseCharacters.SingleOrDefault(x => x.Id == fixedTarget);
                        if(targetChar == null)
                        {
                            return NotFound($"No character found for ID \"{fixedTarget}\"");
                        }

                        targetChars.Add(targetChar);
                    }
                }
            }

            RollResult? damage = null;
            if (!ability.Damage.IsNullOrEmpty())
            {
                damage = RollAuxiliary.CharRollString(ability.Damage!, c);
            }

            List<Message> returnMessages = new List<Message>();
            if(!ability.Self.IsNullOrEmpty())
            {
                RollResult self = RollAuxiliary.CharRollString(ability.Self!, c);

                if(!ability.Target.IsNullOrEmpty())
                {
                    if(ability.TargetType == TargetType.Self)
                    {
                        RollResult target = RollAuxiliary.CharRollString(ability.Target!, c);

                        string color = "Red";
                        if(self.Result >= target.Result)
                        {
                            color = "Green";
                        }

                        returnMessages.Add(new Message($"Rolled {self.Result} out of {target.Result}", $"Roll:\n{self.Detail}\n\nTarget:\n{target.Detail}", color));

                        if (damage != null)
                        {
                            returnMessages.Add(new Message($"Damage - {damage.Result}", damage.Detail));
                        }
                    }
                    else
                    {
                        foreach(Character target in targetChars)
                        {
                            RollResult targetRoll = RollAuxiliary.CharRollString(ability.Target!, target);

                            Message message = new Message($"{c.Name} (Id {c.Id}) rolled {self.Result} out of {targetRoll.Result} against {target.Name} (Id {target.Id})", $"Roll:\n{self.Detail}\n\nAgainst:\n{targetRoll.Detail}", "Red");
                            if (self.Result >= targetRoll.Result)
                            {
                                message.Color = "Green";

                                if(self.Result >= (targetRoll.Result + 10)) {
                                    message.Title += "\nApply Injury if relevant";
                                }
                            }


                            if(self.Result >= targetRoll.Result)
                            {
                                if(damage != null)
                                {
                                    target.Hp -= damage.Result;
                                    message.Description += $"\n\nDamage: {damage.Result}\n{damage.Detail}";
                                }

                                if(ability.EffectTemplates.Count > 0)
                                {
                                    message.Description += $"\n\nEffects: ";
                                }
                                foreach(EffectTemplate template in ability.EffectTemplates)
                                {
                                    target.Effects.Add(template.Instantiate());
                                    message.Description += $"\"{template.Name}\" ";
                                }
                            }

                            returnMessages.Add(message);
                        }
                    }
                }
                else
                {
                    returnMessages.Add(new Message($"Self - {self.Result}", self.Detail)); 

                    if (damage != null)
                    {
                        returnMessages.Add(new Message($"Damage - {damage.Result}", damage.Detail));
                    }
                }
            }
            else
            {
                Message message = new Message("Applied ", "");
                if(damage != null)
                {
                    foreach (Character target in targetChars)
                    {
                        target.Hp -= damage.Result;
                    }
                    message.Title += $"{damage.Result} Damage ";
                }

                if(damage != null && ability.EffectTemplates.Count > 0)
                {
                    message.Title += "and ";
                }

                if(ability.EffectTemplates.Count > 0)
                {
                    message.Title += "Effects ";
                }

                foreach(EffectTemplate template in ability.EffectTemplates)
                {
                    message.Title += $"\"{template.Name}\" ";
                    foreach(Character target in targetChars)
                    {
                        target.Effects.Add(template.Instantiate());
                    }
                }

                message.Title += "to all targets!";
                message.Description += $"Damage:\n{damage.Detail}\n";
                message.Description += "\nTargets:\n";
                foreach(Character target in targetChars)
                {
                    message.Description += $"\"{target.Name}\" (Id {target.Id}), ";
                }
                message.Description = message.Description[..^2];
                returnMessages.Add(message);
            }

            foreach(Roll roll in ability.Rolls!)
            {
                RollResult rollResult = RollAuxiliary.CharRollString(roll.Instruction, c);
                returnMessages.Add(new Message($"Additional Roll \"{roll.Name}\" - {rollResult.Result}", rollResult.Detail));
            }

            _context.SaveChanges();

            return returnMessages;
        }

        private IQueryable<Character> GetBaseCharacters()
        {
            return _context.Characters
                    .Include(x => x.Counters)
                    .Include(x => x.Items).ThenInclude(x => x.Counters)
                    .Include(x => x.Effects).ThenInclude(x => x.Counters)
                    .Include(x => x.Effects).ThenInclude(x => x.EffectCounter)
                    .Include(x => x.Traits).ThenInclude(x => x.Counters)
                    .Include(x => x.Stats).ThenInclude(x => x.Stat);
        }
    }
}
