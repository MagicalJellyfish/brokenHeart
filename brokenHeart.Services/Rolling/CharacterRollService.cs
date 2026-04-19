using brokenHeart.DB;
using brokenHeart.Models.Rolling;

namespace brokenHeart.Services.Rolling
{
    public class CharacterRollService : ICharacterRollService
    {
        private readonly BrokenDbContext _context;
        private readonly IRollService _rollService;

        private static List<char> operators = new List<char> { '+', '-', '*', '/', '(', ')' };

        public CharacterRollService(BrokenDbContext context, IRollService rollService)
        {
            _context = context;
            _rollService = rollService;
        }

        public List<RollResult> CharRollString(string input, int charId, int repeat)
        {
            string replaced = ReplaceVariables(input, GetRollableCharacter(charId));

            List<RollResult> results = new List<RollResult>();
            for (int i = 0; i < repeat; i++)
            {
                results.Add(_rollService.RollString(replaced, input));
            }

            return results;
        }

        public RollResult CharRollString(string input, int charId)
        {
            string replaced = ReplaceVariables(input, GetRollableCharacter(charId));

            return _rollService.RollString(replaced, input);
        }

        private RollableCharacter GetRollableCharacter(int charId)
        {
            RollableCharacter? character = _context
                .Characters.Where(x => x.Id == charId)
                .Select(
                    (x) =>
                        new RollableCharacter()
                        {
                            Id = x.Id,
                            Hp = x.Hp,
                            Armor = x.Armor,
                            Evasion = x.Evasion,
                            MovementSpeed = x.MovementSpeed,
                            Stats = x
                                .Stats.Select(x => new KeyValuePair<string, int>(
                                    x.Stat.Name,
                                    x.Value
                                ))
                                .ToList(),
                            Counters = x
                                .Counters.Select(c => new KeyValuePair<string, int>(
                                    c.Name,
                                    c.Value
                                ))
                                .ToList(),
                            Variables = x
                                .Variables.Select(v => new KeyValuePair<string, int>(
                                    v.Name,
                                    v.Value
                                ))
                                .ToList(),
                        }
                )
                .SingleOrDefault();

            if (character == null)
            {
                throw new Exception($"No character found with id {charId}");
            }

            return character;
        }

        private string ReplaceVariables(string input, RollableCharacter c)
        {
            string output = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (operators.Contains(input[i]) || int.TryParse(input[i].ToString(), out _))
                {
                    output += input[i];
                }
                else
                {
                    string value = "";
                    if (input[i] != '[')
                    {
                        int end;
                        for (end = 0; end < input.Length - i; end++)
                        {
                            if (
                                operators.Contains(input[i + end])
                                || int.TryParse(input[i + end].ToString(), out _)
                            )
                            {
                                break;
                            }
                        }

                        value = input[i..(i + end)];
                    }
                    //Legacy functionality required putting variables in square brackets f.e. 1d20+[DEX]
                    else
                    {
                        value = input[(i + 1)..].Split(']').First();
                        i += 2;
                    }

                    KeyValuePair<string, int> statEntry = c.Stats.SingleOrDefault(kvp =>
                        kvp.Key.ToLower()[0..3] == value.ToLower()
                    );

                    if (!statEntry.Equals(default(KeyValuePair<string, int>)))
                    {
                        output += statEntry.Value;
                        i += 2;
                        continue;
                    }

                    if (value.StartsWith("c:"))
                    {
                        string counterName = value.Substring(2);
                        KeyValuePair<string, int> counterEntry = c.Counters.SingleOrDefault(kvp =>
                            kvp.Key.ToLower() == counterName.ToLower()
                        );

                        if (!counterEntry.Equals(default(KeyValuePair<string, int>)))
                        {
                            output += counterEntry.Value;
                            i += (1 + counterName.Length);
                            continue;
                        }
                        else
                        {
                            throw new Exception($"No counter found with name \"{counterName}\"");
                        }
                    }

                    if (value.StartsWith("v:"))
                    {
                        string variableName = value.Substring(2);
                        KeyValuePair<string, int> variableEntry = c.Variables.SingleOrDefault(kvp =>
                            kvp.Key.ToLower() == variableName.ToLower()
                        );

                        if (!variableEntry.Equals(default(KeyValuePair<string, int>)))
                        {
                            output += variableEntry.Value;
                            i += (1 + variableName.Length);
                            continue;
                        }
                        else
                        {
                            throw new Exception($"No variable found with name \"{variableName}\"");
                        }
                    }

                    switch (value.ToLower())
                    {
                        //Character variables
                        case "hp":
                            output += c.Hp;
                            i += 1;
                            break;
                        case "arm":
                            output += c.Armor;
                            i += 2;
                            break;
                        case "eva":
                            output += c.Evasion;
                            i += 2;
                            break;
                        case "def":
                            output += (c.Armor + c.Evasion);
                            i += 2;
                            break;
                        case "mov":
                            output += c.MovementSpeed;
                            i += 2;
                            break;
                        //If nothing fits, it was probably 'd' for dice
                        default:
                            output += input[i];
                            break;
                    }
                }
            }

            return output;
        }
    }
}
