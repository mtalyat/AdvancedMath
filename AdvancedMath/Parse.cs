using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    public static class Parse
    {
        /*
         * In the future...
         * 
         * [ ] used for matrices
         * { } used for Laplace transforms
         * < > used for vectors
         * ( ) used for parenthesis
         */
        private static string brackets = "()";

        private static string operators = "+-*/^%!";

        public static string Brackets
        {
            get => brackets;
            set
            {
                if (value.Length % 2 == 1)
                    throw new ArgumentException("The Brackets string must be an even amount of characters.");

                brackets = value;
            }
        }

        /// <summary>
        /// Parses a string into a Token.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Token ParseString(string str)
        {
            Expression expr = ParseExpression(SplitString(str));

            //if the expression only has one term, extract that term
            return expr.Reduce();
        }

        #region Parsing

        /// <summary>
        /// Parses an Element from the string at index i.
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static Element ParseElement(string[] strs, ref int i)
        {
            //the output element
            Element currentElement;

            //the first character of the current token
            string current = strs[i];
            char c = current[0];

            //used to parse numbers
            double val;

            if (IsOpeningBracket(c))
            {
                //find the closing bracket
                int closingIndex = FindNextBracket(strs, i + 1, current[0]);

                //get the string tokens in between the two brackets
                string[] sub = SubArray(strs, i + 1, closingIndex - 1);

                //set to the current token so we can decide what operation to do later
                Token inside = ParseExpression(sub).Reduce();

                if (inside is Term t)
                {
                    inside = new Expression(t);
                }

                currentElement = (Element)inside;

                //set the next i to the closing bracket,
                //so that the next iteration starts after that
                i = closingIndex;
            }
            else if (IsClosingBracket(c))
            {
                //opening brackets should handle closing brackets, so if it got here, that means there was a closing bracket
                //that is missing an opening bracket
                throw new ParsingException($"Mismatch brackets: The opening bracket for {c} could not be found.");
            }
            else if (double.TryParse(current, out val))
            {
                //ope, it is a number
                Number n = new Number(val);

                currentElement = n;
            }
            else if (char.IsLetter(c))
            {
                //could be a variable, constant or a function name
                Constant constant;
                Function function;

                //first check for constant
                if (Constant.TryGetConstant(current, out constant))
                {
                    //it is a constant
                    currentElement = constant;
                }
                else if (Functions.TryGetFunction(current, out function))
                {
                    //the next token should be an open parenthesis
                    int closingIndex = FindNextBracket(strs, i + 2, strs[i + 1][0]);

                    string[] sub = SubArray(strs, i + 2, closingIndex - 1);

                    //split the arguments up by commas
                    for (int j = 0; j < sub.Length; j++)
                    {
                        int next = j;
                        for (; next < sub.Length; next++)
                        {
                            if (sub[next][0] == Tokens.SEPARATOR)
                            {
                                break;
                            }
                        }

                        function.AddArgument(ParseExpression(SubArray(sub, j, next - 1)));

                        j = next;
                    }

                    i = closingIndex;

                    //it is a function
                    currentElement = function;
                }
                else if (current.Length == 1 || current[1] == '_')
                {
                    //it is a variable
                    string[] split = current.Split('_');

                    if (split.Length == 1)
                    {
                        currentElement = new Variable(split[0][0]);
                    }
                    else
                    {
                        currentElement = new Variable(split[0][0], uint.Parse(split[1]));
                    }
                }
                else
                {
                    //not sure what it was
                    throw new ParsingException($"Unknown Token: \"{current}\"");
                }
            }
            else
            {
                //not sure what it was
                throw new ParsingException($"Unknown Token: \"{current}\"");
            }

            return currentElement;
        }

        /// <summary>
        /// Parses an Expression from the given array of string tokens.
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        private static Expression ParseExpression(string[] strs)
        {
            //go through each string token and determine what it is, and how it should be organized

            /*
             * Numbers are pretty self explanatory
             * Constants are as well
             * 
             * When a parenthesis/bracket is hit, everything inside of it goes into an expression.
             */

            if (!strs.Any()) return Expression.Zero;

            Expression output = new Expression();

            Element currentElement;
            Term currentTerm = null;

            char operation = ' ';

            bool nextOperandIsNegative = false;

            string current;
            char c;

            for (int i = 0; i < strs.Length; i++)
            {
                current = strs[i];
                c = current[0];

                if (IsOperator(c))
                {
                    //if it is an operator, set the next operation

                    //catch negative signs only
                    if (c == '-' && operation != ' ')
                    {
                        nextOperandIsNegative = true;
                    }
                    else if (operation == ' ')
                    {
                        //normal operation
                        operation = c;
                    }
                    else
                    {
                        //this means that we had two operators in a row, such as "/ +"
                        throw new ParsingException("Too many operators in succession.", $"{operation} and {c}");
                    }

                    //nothing else can be done, so just move on to the next iteration
                    continue;
                }

                //everything else is an operand of some sort
                currentElement = ParseElement(strs, ref i);

                //now decide what to do based on the operation

                //right associative operations
                while(i < strs.Length - 2 && strs[i + 1][0] == '^')
                {
                    //if there is enough room for an operation and an operand, at least, then check if the next operation is right associative, then decide what to do

                    //move i to the element after the ^
                    i += 2;

                    bool isNegative = strs[i][0] == '-';

                    if (isNegative)
                    {
                        i++;
                    }

                    Element temp = ParseElement(strs, ref i);

                    if(isNegative)
                    {
                        temp = (Element)temp.Multiply(Number.NegativeOne);
                    }

                    //now raise the current element by the exponent
                    currentElement = new Term.TermElement(currentElement, temp);
                }

                //if the left has not been filled, fill that
                if (currentTerm == null)
                {
                    currentTerm = new Term(currentElement);

                    //if the operation is a -, but there is no term before to use it with, that means this term is negative
                    if (operation == '-')
                    {
                        currentTerm = (Term)currentTerm.Multiply(Number.NegativeOne);
                        operation = ' ';//clear the operation, since it technically did it's job
                    }

                    continue;
                }

                //after right associative operators have been dealth with, then left associative can act as normal

                //left associative operations
                switch (operation)
                {
                    case '+'://adding, which implies that the last term is complete, so it can be added to the output
                        output = (Expression)output.Add(currentTerm);
                        currentTerm = new Term(currentElement);//new token becomes the working term
                        break;
                    case '-'://subtraction, which implies that the last term is complete, so it can be added to the output. The next element is negative
                        output = (Expression)output.Add(currentTerm);
                        currentTerm = new Term(Number.NegativeOne, currentElement);
                        break;
                    case '*'://multiplication, which implies that the new token should be multiplied into the term
                    case ' '://if there was no operator, we assume that it was a multiplication                    
                        currentTerm = (Term)currentTerm.Multiply(currentElement);
                        break;
                    case '/'://division, which is multiplication, except the new token needs to be on the denominator
                        currentTerm = (Term)currentTerm.Multiply(Term.CreateFraction(Number.One, currentElement));
                        break;

                    //TODO: function mod() and function fact() for % and !

                    default://no idea what this is
                        throw new ParsingException($"Unimplemented operator.", operation.ToString());
                }

                //handle negativity
                if (nextOperandIsNegative)
                {
                    currentTerm = (Term)currentTerm.Multiply(Number.NegativeOne);
                    nextOperandIsNegative = false;
                }

                //we are done, so clear the current element and operation
                operation = ' ';
            }

            //operation should be cleared by the end, so if we have an operation left over, an error has occured
            if(operation != ' ')
            {
                throw new ParsingException("Unmatched operator at end of line.", operation.ToString());
            }

            //add the current term to the output
            output = (Expression)output.Add(currentTerm);

            //return the output
            return output;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a sub array from the given array, using the inclusive start and end indices.
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static string[] SubArray(string[] strs, int start, int end)
        {
            int length = end - start + 1;

            string[] output = new string[length];

            Array.Copy(strs, start, output, 0, length);

            return output;
        }

        /// <summary>
        /// Finds the next corresponding bracket to the given opening bracket, starting at the index start.
        /// </summary>
        /// <param name="strs"></param>
        /// <param name="start"></param>
        /// <param name="open"></param>
        /// <returns></returns>
        private static int FindNextBracket(string[] strs, int start, char open)
        {
            int depth = 0;
            char c;

            for (int i = start; i < strs.Length; i++)
            {
                c = strs[i][0];

                //if another opening bracket, increase the depth
                if (IsOpeningBracket(c))
                {
                    //if an opening bracket, increase the depth
                    depth++;
                }
                else if (IsClosingBracket(c))
                {
                    //if a closing bracket, and the depth is zero, the brackets should match
                    if (depth == 0)
                    {
                        if (!IsMatchingBrackets(open, c))
                        {
                            throw new ParsingException($"Mismatch brackets: {open} does not match {c}.");
                        }
                        else
                        {
                            return i;
                        }
                    }

                    depth--;
                }
            }

            //if it got this far, it could not find the closing bracket
            throw new ParsingException($"Mismatch brackets: The closing bracket for {open} could not be found.");
        }

        /// <summary>
        /// Splits a string into managable parts. Each part contains a "token", whether it is a number, variable, function name, parenthesis, operator, etc.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string[] SplitString(string str)
        {
            List<string> output = new List<string>();

            char c;
            char lastC = ' ';

            //get rid of spaces and double negatives
            str = str.Replace(" ", "").Replace("--", "+");

            if (str == "") return new string[0];

            StringBuilder current = new StringBuilder();

            for (int i = 0; i < str.Length; i++)
            {
                c = str[i];

                //we don't want whitespace, as it does not make a difference
                if (char.IsWhiteSpace(c)) continue;

                //if this character is a bracket OR an operator OR
                //this character is a letter, and the current token starts with a number OR
                //the last character was an operator or a bracket, and this character is a letter or digit
                if (IsBracket(c) || IsOperator(c) ||
                    (char.IsLetter(c) && current.Length > 0 && (current[0] == '-' || char.IsDigit(current[0]))) ||
                    ((IsOperator(lastC) || IsBracket(lastC)) && char.IsLetterOrDigit(c)))
                {
                    //if it is a bracket, put whatever has been accumulating into a token
                    //if it is an operator, put current into the output and keep going

                    //if this is a letter, and the beginning of current is '-' or a number, then this is a separate thing

                    //only add if there is something there
                    if(current.Length > 0)
                    {
                        output.Add(current.ToString());
                        current.Clear();
                    }
                }

                //everything else can just be added regardless of what is in front

                //if it is a letter, it could be a function name or a variable, so we just add it anyways
                //if it is a number, and the first char of current is a letter, this is part of a function name
                //if it is a number, and the first char of current is '-' or a number, this is part of a number
                //either way, add it
                current.Append(c);

                lastC = c;
            }

            //add the remaining current to the output
            output.Add(current.ToString());

            //lastly, return the split up string
            return output.ToArray();
        }

        #region Char Determination

        /// <summary>
        /// Determines if the given char is an operator.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsOperator(char c)
        {
            return operators.Contains(c);
        }

        /// <summary>
        /// Determines if the given char is an opening bracket.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsOpeningBracket(char c)
        {
            int index = brackets.IndexOf(c);

            return index >= 0 && index % 2 == 0;
        }

        /// <summary>
        /// Determines if the given char is a closing bracket.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsClosingBracket(char c)
        {
            int index = brackets.IndexOf(c);

            return index >= 0 && index % 2 == 1;
        }

        /// <summary>
        /// Determines if the given char is a bracket.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsBracket(char c)
        {
            return brackets.Contains(c);
        }

        /// <summary>
        /// Determines if the given left and right chars are matching brackets.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static bool IsMatchingBrackets(char left, char right)
        {
            int index = brackets.IndexOf(left);

            //if the left bracket does not exist/is at an odd index, it is not a bracket, or not suppposed to be on the left
            if (index == -1 || index % 2 == 1) return false;

            //otherwise make sure the left matches the right
            return brackets[index + 1] == right;
        }

        #endregion

        #endregion
    }
}
