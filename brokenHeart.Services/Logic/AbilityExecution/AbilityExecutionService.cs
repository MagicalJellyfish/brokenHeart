using brokenHeart.Database.DAO;
using brokenHeart.Database.DAO.Abilities;
using brokenHeart.Database.DAO.Abilities.Abilities;
using brokenHeart.Database.DAO.Combat;
using brokenHeart.DB;
using brokenHeart.Models.brokenHand;
using brokenHeart.Models.Rolling;
using brokenHeart.Services.DataTransfer.Save;
using brokenHeart.Services.Rolling;
using Microsoft.AspNetCore.Mvc;

namespace brokenHeart.Services.Logic.AbilityExecution
{
    internal class AbilityExecutionService : IAbilityExecutionService
    {
        private readonly BrokenDbContext _context;
        private readonly ICharacterRollService _charRollService;
        private readonly IElementSubmissionService _elementSubmissionService;

        public AbilityExecutionService(
            BrokenDbContext context,
            ICharacterRollService charRollService,
            IElementSubmissionService elementSubmissionService
        )
        {
            _context = context;
            _charRollService = charRollService;
            _elementSubmissionService = elementSubmissionService;
        }

        public ActionResult<List<Message>> ExecuteAbility(
            int charId,
            string shortcut,
            string? targets,
            string? selfModifer,
            string? targetModifier,
            string? damageModifier
        )
        {
            var executingChar = _context
                .Characters.Where(x => x.Id == charId)
                .Select(x => new { x.Id, x.Name })
                .SingleOrDefault();

            if (executingChar == null)
            {
                throw new Exception($"No character found with ID {charId}!");
            }

            Ability ability = DetermineAbility(charId, shortcut);

            if (ability.MaxUses != 0)
            {
                if (ability.Uses == 0)
                {
                    throw new Exception("Cannot use ability which has 0 uses left!");
                }

                ability.Uses -= 1;
            }

            RollResult? damage = null;
            if (!string.IsNullOrEmpty(ability.Damage))
            {
                string damageString = ability.Damage!;
                if (damageModifier != null)
                {
                    damageString += damageModifier;
                }

                damage = _charRollService.CharRollString(damageString, executingChar.Id);
            }

            List<Message> returnMessages = new();
            if (!string.IsNullOrEmpty(ability.Self))
            {
                string selfString = ability.Self!;
                if (selfModifer != null)
                {
                    selfString += selfModifer;
                }

                RollResult self = _charRollService.CharRollString(selfString, executingChar.Id);

                if (!string.IsNullOrEmpty(ability.Target))
                {
                    if (ability.TargetType == TargetType.Self)
                    {
                        returnMessages.Add(
                            ApplySelfAbility(
                                executingChar.Id,
                                ability,
                                self,
                                damage,
                                targetModifier
                            )
                        );
                    }
                    else
                    {
                        returnMessages.AddRange(
                            ApplyRollTargetAbility(
                                executingChar.Id,
                                executingChar.Name,
                                ability,
                                targets,
                                self,
                                damage,
                                targetModifier
                            )
                        );
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
                returnMessages.AddRange(ApplyDirectTargetAbility(ability, targets, damage));
            }

            foreach (Roll roll in _context.Rolls.Where(x => x.AbilityId == ability.Id))
            {
                RollResult rollResult = _charRollService.CharRollString(
                    roll.Instruction,
                    executingChar.Id
                );
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

        private Ability DetermineAbility(int charId, string shortcut)
        {
            IQueryable<Character> character = _context.Characters.Where(x => x.Id == charId);
            IQueryable<Ability> allAbilities = character
                .SelectMany(x => x.Abilities)
                .Concat(character.SelectMany(x => x.Items).SelectMany(x => x.Abilities))
                .Concat(character.SelectMany(x => x.Traits).SelectMany(x => x.Abilities))
                .Concat(character.SelectMany(x => x.Effects).SelectMany(x => x.Abilities));

            List<Ability> targetAbilities = allAbilities
                .Where(x => x.Shortcut.ToLower() == shortcut.ToLower())
                .ToList();

            if (targetAbilities.Count() != 1)
                throw new Exception(
                    $"Found {targetAbilities.Count()} abilities with shortcut \"{shortcut}\"! Conflicting abilities are: {string.Join(", ", targetAbilities.Select(x => x.Name))}"
                );

            return targetAbilities.Single();
        }

        private IQueryable<Character> DetermineTargets(string? targets)
        {
            List<int> targetIds = new();

            if (!string.IsNullOrEmpty(targets))
            {
                IQueryable<Character> characters = _context.Characters;

                List<string> targetList = targets!.Split(' ').ToList();
                foreach (string target in targetList)
                {
                    //If just numbers it's a char's ID, otherwise shortcut
                    if (int.TryParse(target, out _))
                    {
                        int fixedTarget = int.Parse(target.Trim());

                        IQueryable<Character> targetChar = _context.Characters.Where(x =>
                            x.Id == fixedTarget
                        );

                        if (targetChar.Count() != 1)
                        {
                            throw new Exception(
                                $"Found {targetChar.Count()} characters for ID \"{fixedTarget}\""
                            );
                        }

                        targetIds.Add(fixedTarget);
                    }
                    else
                    {
                        IQueryable<Combat>? activeCombat = _context.Combats.Where(x => x.Active);
                        if (activeCombat.Count() != 1)
                        {
                            throw new Exception($"Found {activeCombat.Count()} active combats!");
                        }

                        string normalizedTarget = target.ToLower().Trim();
                        IQueryable<Character> targetChar = activeCombat
                            .SelectMany(x => x.Entries)
                            .Where(x =>
                                x.Character != null
                                && x.Shortcut.ToLower().Trim() == normalizedTarget
                            )
                            .Select(x => x.Character!);
                        if (targetChar.Count() != 1)
                        {
                            throw new Exception(
                                $"Found {targetChar.Count()} targets with shortcut \"{normalizedTarget}\" in combat!"
                            );
                        }

                        targetIds.Add(targetChar.Single().Id);
                    }
                }
            }

            return _context.Characters.Where(c => targetIds.Contains(c.Id));
        }

        private bool EvaluateHit(RollResult self, RollResult target)
        {
            if (self.CriticalFailure)
                return false;

            if (target.CriticalFailure)
                return true;

            if (self.CriticalSuccess && !target.CriticalSuccess)
                return true;

            if (!self.CriticalSuccess && target.CriticalSuccess)
                return false;

            if (self.Result >= target.Result)
                return true;

            return false;
        }

        private Message ApplySelfAbility(
            int charId,
            Ability ability,
            RollResult self,
            RollResult? damage,
            string? targetModifier
        )
        {
            string targetString = ability.Target!;
            if (targetModifier != null)
            {
                targetString += targetModifier;
            }

            RollResult target = _charRollService.CharRollString(targetString, charId);

            Message message = new Message(
                $"Rolled {self.Result} out of {target.Result}",
                $"Roll:\n{self.Detail}\n\nTarget:\n{target.Detail}",
                "Red"
            );

            if (EvaluateHit(self, target))
            {
                message.Color = "Green";

                if (damage != null)
                {
                    Character selfCharacter = _context.Characters.Single(x => x.Id == charId);
                    selfCharacter.Hp -= damage.Result;
                    message.Description += $"\n\nDamage: {damage.Result}\n{damage.Detail}";
                }
            }

            return message;
        }

        private List<Message> ApplyRollTargetAbility(
            int executingCharId,
            string executingCharName,
            Ability ability,
            string? targets,
            RollResult self,
            RollResult? damage,
            string? targetModifier
        )
        {
            List<Message> messages = new();

            IQueryable<Character> targetChars = DetermineTargets(targets);
            foreach (Character target in targetChars)
            {
                string targetRollString = ability.Target!;
                if (targetModifier != null)
                {
                    targetRollString += targetModifier;
                }

                RollResult targetRoll = _charRollService.CharRollString(
                    targetRollString,
                    target.Id
                );

                Message message = new Message(
                    $"{executingCharName} (Id {executingCharId}) rolled {self.Result} out of {targetRoll.Result} against {target.Name} (Id {target.Id})",
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
                        message.Description += $"\n\nDamage: {damage.Result}\n{damage.Detail}";
                    }

                    var effectTemplates = _context
                        .EffectTemplates.Where(x =>
                            x.ApplyingAbilities.Any(a => a.Id == ability.Id)
                        )
                        .Select(x => new { x.Id, x.Name });

                    if (effectTemplates.Count() > 0)
                        message.Description += $"\n\nEffects: ";

                    foreach (var template in effectTemplates)
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

                messages.Add(message);
            }

            return messages;
        }

        private List<Message> ApplyDirectTargetAbility(
            Ability ability,
            string? targets,
            RollResult? damage
        )
        {
            List<Message> messages = new();

            IQueryable<Character> targetChars = DetermineTargets(targets);

            var effectTemplates = _context
                .EffectTemplates.Where(x => x.ApplyingAbilities.Any(a => a.Id == ability.Id))
                .Select(x => new { x.Id, x.Name });

            if (damage != null || effectTemplates.Count() != 0)
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

                if (damage != null && effectTemplates.Count() > 0)
                {
                    message.Title += "and ";
                }

                if (effectTemplates.Count() > 0)
                {
                    message.Title += "Effects ";
                }

                foreach (var template in effectTemplates)
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
                        message.Description += $"\n\"{target.Name}\" (Id {target.Id}) is at 0 HP!";
                    }
                }
                messages.Add(message);
            }
            else
            {
                Message message = new Message(
                    "Executed ability!",
                    $"Executed ability {ability.Name}!"
                );

                messages.Add(message);
            }

            return messages;
        }
    }
}
