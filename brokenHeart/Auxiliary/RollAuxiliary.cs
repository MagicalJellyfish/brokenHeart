using NuGet.Protocol;

namespace brokenHeart.Auxiliary
{
    public class RollAuxiliary
    {
        private static Random rnd = new Random();

        public enum KeepType
        {
            Highest,
            Lowest,
            None
        }

        public static RollResult RollString(string input)
        {
            input = input.Replace(" ", "");

            List<char> specialChars = new List<char> { 'd', '+', '-', '*', '/', '(', ')' };

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
                        if (specialChars.Contains(input[post]))
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
                        else
                        {
                            postD += input[post];
                        }
                    }

                    if (!foundK)
                    {
                        i = post - 1;
                    }

                    RollResult rollReturn = Roll(int.Parse(preD), int.Parse(postD), keepType, int.Parse(keepNum));
                    rolledString += rollReturn.Result;
                    detailString += rollReturn.Detail;
                }
            }


            int result = EvaluateString(rolledString);

            return new RollResult(result, detailString);
        }

        private static int EvaluateString(string input)
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
                        firstNum = Calculate(firstNum, operation, secondNum).ToString();

                        operation = parsedInput[i];
                        secondNum = "";
                    }
                    else
                    {
                        operation = parsedInput[i];
                        operatorFound = true;
                    }
                }
            }

            int result;
            if (operation != ' ')
            {
                result = Calculate(firstNum, operation, secondNum);
            }
            else
            {
                result = int.Parse(firstNum);
            }

            return result;
        }

        private static int Calculate(string first, char operation, string second)
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

        public static RollResult Roll(int rolls, int die, KeepType keep = KeepType.None, int keepNum = -1)
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
}
