﻿using brokenHeart.Database.DAO;
using brokenHeart.Database.DAO.Characters;
using brokenHeart.Database.DAO.Counters;
using brokenHeart.Database.DAO.Stats;
using brokenHeart.Models.Rolling;

namespace brokenHeart.Services.Rolling
{
    public class RollService : IRollService
    {
        private static Random rnd = new Random();
        private static List<char> operators = new List<char> { '+', '-', '*', '/', '(', ')' };
        private static List<char> specialChars = new List<char>(operators) { 'd' };

        public RollService() { }

        public List<RollResult> CharRollString(string input, Character c, int repeat)
        {
            string replaced = ReplaceVariables(input, c);

            List<RollResult> results = new List<RollResult>();
            for (int i = 0; i < repeat; i++)
            {
                results.Add(RollString(replaced, input));
            }

            return results;
        }

        public RollResult CharRollString(string input, Character c)
        {
            string replaced = ReplaceVariables(input, c);

            return RollString(replaced, input);
        }

        private string ReplaceVariables(string input, Character c)
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

                    StatValue? statValue = c.Stats.SingleOrDefault(x =>
                        x.Stat.Name.ToLower()[0..3] == value.ToLower()
                    );
                    if (statValue != null)
                    {
                        output += statValue.Value;
                        i += 2;
                        continue;
                    }

                    if (value.StartsWith("c:"))
                    {
                        string counterName = value.Substring(2);
                        Counter? counter = c.Counters.SingleOrDefault(x =>
                            x.Name.ToLower().Equals(counterName.ToLower())
                        );
                        if (counter != null)
                        {
                            output += counter.Value;
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
                        Variable? variable = c.Variables.SingleOrDefault(x =>
                            x.Name.ToLower().Equals(variableName.ToLower())
                        );
                        if (variable != null)
                        {
                            output += variable.Value;
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

        public List<RollResult> RollString(string input, string? original = null, int repeat = 1)
        {
            List<RollResult> results = new List<RollResult>();
            for (int i = 0; i < repeat; i++)
            {
                results.Add(RollString(input, original));
            }

            return results;
        }

        public RollResult RollString(string input, string? original = null)
        {
            bool critEvaluated = false;
            bool criticalSuccess = false;
            bool criticalFailure = false;

            input = input.Replace(" ", "");

            string detailString = "";
            string rolledString = "";

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != 'd')
                {
                    detailString += input[i];
                    rolledString += input[i];
                }
                else
                {
                    bool preFin = false;
                    string preD = "";
                    for (int pre = i - 1; !preFin && pre >= 0; pre--)
                    {
                        if (specialChars.Contains(input[pre]))
                        {
                            preFin = true;
                        }
                        else
                        {
                            preD = input[pre] + preD;
                            detailString = detailString[..^1];
                            rolledString = rolledString[..^1];
                        }
                    }

                    if (preD == "")
                    {
                        preD = "1";
                    }

                    KeepType keepType = KeepType.None;
                    string keepNum = "-1";
                    string postD = "";
                    bool foundK = false;
                    int post;
                    for (post = i + 1; post < input.Length; post++)
                    {
                        if (operators.Contains(input[post]))
                        {
                            break;
                        }
                        else if (input[post] == 'k')
                        {
                            foundK = true;

                            switch (input[post + 1])
                            {
                                case 'h':
                                    keepType = KeepType.Highest;
                                    break;
                                case 'l':
                                    keepType = KeepType.Lowest;
                                    break;
                            }

                            keepNum = "";
                            for (int postKeep = post + 2; postKeep < input.Length; postKeep++)
                            {
                                if (specialChars.Contains(input[postKeep]))
                                {
                                    break;
                                }
                                else
                                {
                                    keepNum += input[postKeep];
                                }

                                i = postKeep;
                            }

                            break;
                        }
                        else if (
                            input.Substring(post).StartsWith("adv")
                            || input.Substring(post).StartsWith("dis")
                        )
                        {
                            foundK = true;

                            switch (input[post..(post + 3)])
                            {
                                case "adv":
                                    keepType = KeepType.Highest;
                                    break;
                                case "dis":
                                    keepType = KeepType.Lowest;
                                    break;
                            }

                            keepNum = preD;
                            preD = (int.Parse(preD) * 2).ToString();

                            i = post + 2;
                            break;
                        }
                        else
                        {
                            postD += input[post];
                        }
                    }

                    if (!foundK)
                    {
                        i = post - 1;
                    }

                    RollResult rollReturn = Roll(
                        int.Parse(preD),
                        int.Parse(postD),
                        keepType,
                        int.Parse(keepNum)
                    );
                    rolledString += rollReturn.Result;
                    detailString += rollReturn.Detail;

                    if (!critEvaluated)
                    {
                        if (int.Parse(preD) == 1 && int.Parse(postD) == 20)
                        {
                            critEvaluated = true;
                            if (rollReturn.Result == 20)
                            {
                                criticalSuccess = true;
                            }
                            if (rollReturn.Result == 1)
                            {
                                criticalFailure = true;
                            }
                        }
                    }
                }
            }

            int result = EvaluateString(rolledString);
            string returnString = "";
            if (original != null)
            {
                returnString += original + "\n= ";
            }

            detailString += $"\n= {result}";

            if (criticalSuccess)
            {
                detailString += "\nCritical Success!";
            }
            else if (criticalFailure)
            {
                detailString += "\nCritical Failure!";
            }
            return new RollResult(
                result,
                returnString + $"{input}\n= {detailString}",
                criticalSuccess,
                criticalFailure
            );
        }

        private int EvaluateString(string input)
        {
            List<char> operators = new List<char> { 'd', '+', '-', '*', '/' };

            string parsedInput = "";
            for (int i = 0; i < input.Length; i++)
            {
                string innerString = "";
                if (input[i] == '(')
                {
                    for (int j = i + 1; j < input.Length; j++)
                    {
                        if (input[j] == ')')
                        {
                            parsedInput += EvaluateString(innerString).ToString();
                            i = j;
                        }
                        else
                        {
                            innerString += input[j];
                        }
                    }
                }
                else
                {
                    parsedInput += input[i];
                }
            }

            string firstNum = "";
            char operation = ' ';
            bool operatorFound = false;
            string secondNum = "";
            for (int i = 0; i < parsedInput.Length; i++)
            {
                if (!operators.Contains(parsedInput[i]))
                {
                    if (!operatorFound)
                    {
                        firstNum += parsedInput[i];
                    }
                    else
                    {
                        secondNum += parsedInput[i];
                    }
                }
                else
                {
                    if (operatorFound)
                    {
                        if (secondNum == "" && (parsedInput[i] == '-' || parsedInput[i] == '+'))
                        {
                            secondNum += parsedInput[i];
                            continue;
                        }

                        firstNum = Calculate(firstNum, operation, secondNum).ToString();

                        operation = parsedInput[i];
                        secondNum = "";
                    }
                    else
                    {
                        if (firstNum == "" && (parsedInput[i] == '-' || parsedInput[i] == '+'))
                        {
                            firstNum += parsedInput[i];
                            continue;
                        }

                        operation = parsedInput[i];
                        operatorFound = true;
                    }
                }
            }

            int result;
            if (operation != ' ')
            {
                if (firstNum == "")
                {
                    firstNum += "-";
                }
                result = Calculate(firstNum, operation, secondNum);
            }
            else
            {
                result = int.Parse(firstNum);
            }

            return result;
        }

        private int Calculate(string first, char operation, string second)
        {
            int firstNum = int.Parse(first);
            int secondNum = int.Parse(second);
            int result = 0;

            switch (operation)
            {
                case '+':
                    result = firstNum + secondNum;
                    break;
                case '-':
                    result = firstNum - secondNum;
                    break;
                case '*':
                    result = firstNum * secondNum;
                    break;
                case '/':
                    result = firstNum / secondNum;
                    break;
            }

            return result;
        }

        public RollResult Roll(int rolls, int die, KeepType keep = KeepType.None, int keepNum = -1)
        {
            if (keepNum == -1)
            {
                keepNum = rolls;
            }

            List<int> diceResults = new List<int>();
            for (int i = 0; i < rolls; i++)
            {
                diceResults.Add(rnd.Next(1, die + 1));
            }

            switch (keep)
            {
                case KeepType.Highest:
                    diceResults = diceResults.OrderByDescending(x => x).ToList();
                    break;
                case KeepType.Lowest:
                    diceResults = diceResults.OrderBy(x => x).ToList();
                    break;
            }

            int resultCombined = 0;
            string detailString = "[";
            for (int i = 0; i < diceResults.Count; i++)
            {
                if (i == keepNum)
                {
                    detailString = detailString[..^3] + " { ";
                }

                if (i < keepNum)
                {
                    resultCombined += diceResults[i];
                    detailString += diceResults[i] + " + ";
                }
                else
                {
                    detailString += diceResults[i] + " / ";
                }

                if (i == diceResults.Count - 1)
                {
                    if (keepNum == rolls)
                    {
                        detailString = detailString[..^3];
                    }
                    else
                    {
                        detailString = detailString[..^3] + " } ";
                    }
                }
            }
            detailString += "]";

            return new RollResult(resultCombined, detailString);
        }
    }

    public enum KeepType
    {
        Highest,
        Lowest,
        None,
    }
}
