using brokenHeart.Database.DAO;
using brokenHeart.Database.DAO.Abilities;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Combat;
using brokenHeart.Database.DAO.Modifiers.Effects;
using brokenHeart.DB;
using brokenHeart.Models.brokenHand;
using brokenHeart.Models.Rolling;
using brokenHeart.Services.DataTransfer.Save;
using brokenHeart.Services.Endpoints;
using brokenHeart.Services.Rolling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace brokenHeart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Localhost]
    public class ActionsController : ControllerBase
    {
        private readonly BrokenDbContext _context;
        private readonly IRollService _rollService;
        private readonly IElementSubmissionService _elementSubmissionService;

        public ActionsController(
            BrokenDbContext brokenDbContext,
            IRollService rollService,
            IElementSubmissionService elementSubmissionService
        )
        {
            _context = brokenDbContext;
            _rollService = rollService;
            _elementSubmissionService = elementSubmissionService;
        }

        [HttpGet("roll")]
        public async Task<ActionResult<RollResult>> Roll(string rollString)
        {
            return _rollService.RollString(rollString);
        }

        [HttpGet("rollChar/{id}")]
        public async Task<ActionResult<RollResult>> RollChar(int id, string rollString)
        {
            Character? c = GetBaseCharacters().SingleOrDefault(x => x.Id == id);

            if (c == null)
            {
                return NotFound("No character found!");
            }

            return _rollService.CharRollString(rollString, c);
        }

        [HttpGet("rollActiveChar/{discordId}")]
        public async Task<ActionResult<RollResult>> RollActiveChar(
            ulong discordId,
            string rollString
        )
        {
            UserSimplified? user = _context
                .UserSimplified.Include(x => x.ActiveCharacter)
                .SingleOrDefault(x => x.DiscordId == discordId);

            if (user == null)
            {
                return NotFound("No user found!");
            }

            if (user.ActiveCharacter == null)
            {
                return NotFound("No active character!");
            }

            return _rollService.CharRollString(
                rollString,
                GetBaseCharacters().Single(x => x.Id == user.ActiveCharacter.Id)
            );
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
            UserSimplified? user = _context
                .UserSimplified.Include(x => x.ActiveCharacter)
                .SingleOrDefault(x => x.DiscordId == discordId);

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

            Character? c;
            if (charId == null)
            {
                if (user.ActiveCharacter == null)
                {
                    return NotFound("No active character!");
                }

                c = GetFullCharacter(user.ActiveCharacter.Id);
            }
            else
            {
                c = GetFullCharacter((int)charId);
            }
            if (c == null)
            {
                return NotFound("No character found!");
            }

            if (targets == null)
            {
                targets = user.DefaultTargetString;
            }

            return ExecuteAbility(
                c,
                shortcut,
                targets,
                selfModifier,
                targetModifier,
                damageModifier
            );
        }

        private ActionResult<List<Message>> ExecuteAbility(
            Character c,
            string shortcut,
            string? targets,
            string? selfModifer,
            string? targetModifier,
            string? damageModifier
        )
        {
            List<Ability> allAbilities = new List<Ability>();
            allAbilities.AddRange(c.Abilities);
            allAbilities.AddRange(c.Items.SelectMany(x => x.Abilities));
            allAbilities.AddRange(c.Traits.SelectMany(x => x.Abilities));
            allAbilities.AddRange(c.Effects.SelectMany(x => x.Abilities));

            Ability? ability;
            try
            {
                ability = allAbilities.SingleOrDefault(x =>
                    x.Shortcut.ToLower() == shortcut.ToLower()
                );
                if (ability == null)
                {
                    return NotFound($"No ability with shortcut \"{shortcut}\" found!");
                }
            }
            catch
            {
                string conflictList = "";
                foreach (var conflictingAbility in allAbilities.Where(x => x.Shortcut == shortcut))
                {
                    conflictList += $"{conflictingAbility.Name}, ";
                }

                return BadRequest($"Shortcuts of abilities {conflictList[..1]} are conflicting!");
            }

            if (ability.MaxUses != 0)
            {
                if (ability.Uses == 0)
                {
                    return BadRequest("Cannot use ability which has 0 uses left!");
                }

                ability.Uses -= 1;
            }

            List<Character> targetChars = new List<Character>();
            if (!string.IsNullOrEmpty(targets))
            {
                List<Character> baseCharacters = GetBaseCharacters().ToList();

                List<string> targetList = targets!.Split(' ').ToList();

                foreach (string target in targetList)
                {
                    //If just numbers it's a char's ID, otherwise shortcut
                    if (int.TryParse(target, out _))
                    {
                        int fixedTarget = int.Parse(target.Trim());

                        Character? targetChar = baseCharacters.SingleOrDefault(x =>
                            x.Id == fixedTarget
                        );
                        if (targetChar == null)
                        {
                            return NotFound($"No character found for ID \"{fixedTarget}\"");
                        }

                        targetChars.Add(targetChar);
                    }
                    else
                    {
                        Combat? activeCombat = _context
                            .Combats.Include(x => x.Entries)
                            .ThenInclude(x => x.Character)
                            .SingleOrDefault(x => x.Active);
                        if (activeCombat == null)
                        {
                            return NotFound("No active combat found!");
                        }

                        List<Character> combatTargets = new List<Character>();
                        combatTargets = activeCombat
                            .Entries.Where(x => x.Character != null)
                            .Select(x => x.Character)
                            .ToList()!;
                        if (combatTargets.Count() == 0)
                        {
                            return NotFound("No potential targets in combat!");
                        }

                        List<Character> fullCombatTargets = baseCharacters
                            .Where(x => combatTargets.Select(y => y.Id).Contains(x.Id))
                            .ToList();

                        string normalizedTarget = target.ToLower().Trim();
                        CombatEntry? entry = activeCombat.Entries.SingleOrDefault(entry =>
                            entry.Shortcut.ToLower().Trim() == normalizedTarget
                            && entry.Character != null
                        );
                        if (entry == null)
                        {
                            return NotFound(
                                $"No combatant found for shortcut \"{normalizedTarget}\""
                            );
                        }

                        Character targetChar = fullCombatTargets.Single(x =>
                            x.Id == entry.Character!.Id
                        );
                        targetChars.Add(targetChar);
                    }
                }
            }

            RollResult? damage = null;
            if (!string.IsNullOrEmpty(ability.Damage))
            {
                string damageString = ability.Damage!;
                if (damageModifier != null)
                {
                    damageString += damageModifier;
                }

                damage = _rollService.CharRollString(damageString, c);
            }

            List<Message> returnMessages = new List<Message>();
            if (!string.IsNullOrEmpty(ability.Self))
            {
                string selfString = ability.Self!;
                if (selfModifer != null)
                {
                    selfString += selfModifer;
                }

                RollResult self = _rollService.CharRollString(selfString, c);

                if (!string.IsNullOrEmpty(ability.Target))
                {
                    if (ability.TargetType == TargetType.Self)
                    {
                        string targetString = ability.Target!;
                        if (targetModifier != null)
                        {
                            targetString += targetModifier;
                        }

                        RollResult target = _rollService.CharRollString(targetString, c);

                        string color = "Red";
                        if (EvaluateHit(self, target))
                        {
                            color = "Green";
                        }

                        returnMessages.Add(
                            new Message(
                                $"Rolled {self.Result} out of {target.Result}",
                                $"Roll:\n{self.Detail}\n\nTarget:\n{target.Detail}",
                                color
                            )
                        );

                        if (damage != null)
                        {
                            returnMessages.Add(
                                new Message($"Damage: {damage.Result}", damage.Detail)
                            );
                        }
                    }
                    else
                    {
                        foreach (Character target in targetChars)
                        {
                            string targetString = ability.Target!;
                            if (targetModifier != null)
                            {
                                targetString += targetModifier;
                            }

                            RollResult targetRoll = _rollService.CharRollString(
                                targetString,
                                target
                            );

                            Message message = new Message(
                                $"{c.Name} (Id {c.Id}) rolled {self.Result} out of {targetRoll.Result} against {target.Name} (Id {target.Id})",
                                $"Roll:\n{self.Detail}\n\nAgainst:\n{targetRoll.Detail}",
                                "Red"
                            );

                            if (EvaluateHit(self, targetRoll))
                            {
                                message.Color = "Green";

                                if (self.Result >= (targetRoll.Result + 10))
                                {
                                    message.Title += "\nApply Injury if relevant";
                                }

                                if (damage != null)
                                {
                                    target.Hp -= damage.Result;
                                    message.Description +=
                                        $"\n\nDamage: {damage.Result}\n{damage.Detail}";
                                }

                                if (ability.AppliedEffectTemplates.Count > 0)
                                {
                                    message.Description += $"\n\nEffects: ";
                                }
                                foreach (EffectTemplate template in ability.AppliedEffectTemplates)
                                {
                                    _elementSubmissionService.InstantiateTemplate(
                                        Models.DataTransfer.ElementType.EffectTemplate,
                                        template.Id,
                                        Models.DataTransfer.ElementType.Character,
                                        target.Id
                                    );
                                    message.Description += $"\"{template.Name}\" ";
                                }
                            }

                            if (target.Hp <= 0)
                            {
                                message.Description += $"\n\n\"{target.Name}\" is at 0 HP!";
                            }
                            returnMessages.Add(message);
                        }
                    }
                }
                else
                {
                    returnMessages.Add(new Message($"Self: {self.Result}", self.Detail));

                    if (damage != null)
                    {
                        returnMessages.Add(new Message($"Damage: {damage.Result}", damage.Detail));
                    }
                }
            }
            else
            {
                if (damage != null || ability.AppliedEffectTemplates.Count != 0)
                {
                    Message message = new Message("Applied ", "");
                    if (damage != null)
                    {
                        foreach (Character target in targetChars)
                        {
                            target.Hp -= damage.Result;
                        }
                        message.Title += $"{damage.Result} Damage ";
                    }

                    if (damage != null && ability.AppliedEffectTemplates.Count > 0)
                    {
                        message.Title += "and ";
                    }

                    if (ability.AppliedEffectTemplates.Count > 0)
                    {
                        message.Title += "Effects ";
                    }

                    foreach (EffectTemplate template in ability.AppliedEffectTemplates)
                    {
                        message.Title += $"\"{template.Name}\" ";
                        foreach (Character target in targetChars)
                        {
                            _elementSubmissionService.InstantiateTemplate(
                                Models.DataTransfer.ElementType.EffectTemplate,
                                template.Id,
                                Models.DataTransfer.ElementType.Character,
                                target.Id
                            );
                        }
                    }

                    message.Title += "to all targets!";
                    message.Description += $"Damage:\n{damage.Detail}\n";
                    message.Description += "\nTargets:\n";
                    foreach (Character target in targetChars)
                    {
                        message.Description += $"\"{target.Name}\" (Id {target.Id}), ";
                    }
                    message.Description = message.Description[..^2];

                    bool firstDownMessage = true;
                    foreach (Character target in targetChars)
                    {
                        if (target.Hp <= 0)
                        {
                            if (firstDownMessage)
                            {
                                firstDownMessage = false;
                                message.Description += "\n";
                            }
                            message.Description +=
                                $"\n\"{target.Name}\" (Id {target.Id}) is at 0 HP!";
                        }
                    }
                    returnMessages.Add(message);
                }
                else
                {
                    Message message = new Message(
                        "Executed ability!",
                        $"Executed ability {ability.Name}!"
                    );

                    returnMessages.Add(message);
                }
            }

            foreach (Roll roll in ability.Rolls!)
            {
                RollResult rollResult = _rollService.CharRollString(roll.Instruction, c);
                returnMessages.Add(
                    new Message(
                        $"Additional Roll \"{roll.Name}\": {rollResult.Result}",
                        rollResult.Detail
                    )
                );
            }

            _context.SaveChanges();

            return returnMessages;
        }

        private bool EvaluateHit(RollResult self, RollResult target)
        {
            if (
                !self.CriticalFailure
                && (
                    self.Result >= target.Result
                    || self.CriticalSuccess && !target.CriticalSuccess
                    || target.CriticalFailure
                )
            )
            {
                return true;
            }

            return false;
        }

        // csharpier-ignore
        private Character? GetFullCharacter(int id)
        {
            return GetBaseCharacters()
                .Include(x => x.Abilities).ThenInclude(x => x.Rolls)
                .Include(x => x.Abilities).ThenInclude(x => x.AppliedEffectTemplates)!.ThenInclude(x => x.CounterTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.Abilities).ThenInclude(x => x.AppliedEffectTemplates)!.ThenInclude(x => x.RoundReminderTemplate)

                .Include(x => x.Items).ThenInclude(x => x.Abilities).ThenInclude(x => x.Rolls)
                .Include(x => x.Items).ThenInclude(x => x.Abilities).ThenInclude(x => x.AppliedEffectTemplates)!.ThenInclude(x => x.CounterTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.Items).ThenInclude(x => x.Abilities).ThenInclude(x => x.AppliedEffectTemplates)!.ThenInclude(x => x.RoundReminderTemplate)

                .Include(x => x.Traits).ThenInclude(x => x.Abilities).ThenInclude(x => x.Rolls)
                .Include(x => x.Traits).ThenInclude(x => x.Abilities).ThenInclude(x => x.AppliedEffectTemplates)!.ThenInclude(x => x.CounterTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.Traits).ThenInclude(x => x.Abilities).ThenInclude(x => x.AppliedEffectTemplates)!.ThenInclude(x => x.RoundReminderTemplate)

                .Include(x => x.Effects).ThenInclude(x => x.Abilities).ThenInclude(x => x.Rolls)
                .Include(x => x.Effects).ThenInclude(x => x.Abilities).ThenInclude(x => x.AppliedEffectTemplates)!.ThenInclude(x => x.CounterTemplates).ThenInclude(x => x.RoundReminderTemplate)
                .Include(x => x.Effects).ThenInclude(x => x.Abilities).ThenInclude(x => x.AppliedEffectTemplates)!.ThenInclude(x => x.RoundReminderTemplate)
                .SingleOrDefault(x => x.Id == id);
        }

        // csharpier-ignore
        private IQueryable<Character> GetBaseCharacters()
        {
            return _context.Characters
                .Include(x => x.Variables)

                .Include(x => x.Counters)

                .Include(x => x.Items).ThenInclude(x => x.Counters)

                .Include(x => x.Effects).ThenInclude(x => x.Counters)
                .Include(x => x.Effects).ThenInclude(x => x.EffectCounter)

                .Include(x => x.Traits).ThenInclude(x => x.Counters)

                .Include(x => x.Stats).ThenInclude(x => x.Stat);
        }
    }
}
