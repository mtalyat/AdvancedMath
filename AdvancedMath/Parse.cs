﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    public static class Parse
    {
        private static string brackets = "()[]{}<>";

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

        public static Token ParseString(string str)
        {
            Expression expr = CompileExpression(SplitString(str));

            //if the expression only has one term, extract that term
            return expr.Reduce();
        }

        private static Expression CompileExpression(string[] strs)
        {
            //go through each string token and determine what it is, and how it should be organized

            /*
             * Numbers are pretty self explanatory
             * Constants are as well
             * 
             * When a parenthesis/bracket is hit, everything inside of it goes into an expression.
             */

            Expression output = new Expression();

            Element currentElement;
            Term currentTerm = null;

            char operation = ' ';

            bool nextOperandIsNegative = false;

            string current;
            char c;
            double val;

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
                    else
                    {
                        //normal operation
                        operation = c;
                    }

                    continue;
                }

                //everything else is an operand of some sort

                if (IsOpeningBracket(c))
                {
                    //find the closing bracket
                    int closingIndex = FindNextBracket(strs, i + 1, current[0]);

                    //get the string tokens in between the two brackets
                    string[] sub = SubArray(strs, i + 1, closingIndex - 1);

                    //set to the current token so we can decide what operation to do later
                    Token inside = CompileExpression(sub).Reduce();

                    if(inside is Term t)
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
                                if(sub[next][0] == Tokens.Separator.ToChar())
                                {
                                    break;
                                }
                            }

                            function.AddArgument(CompileExpression(SubArray(sub, j, next - 1)));

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

                //now decide what to do based on the operation

                //or if the left has not been filled, fill that
                if (currentTerm == null)
                {
                    currentTerm = new Term(currentElement);
                    continue;
                }

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
                    case '^'://power, so just put the current term in another term, with this element as the exponent 
                        currentTerm = Term.RaisePower(currentTerm, currentElement);
                        break;

                    //TODO: function mod() and function fact() for % and !

                    default://no idea what this is
                        throw new NotImplementedException($"Unknown operator: {operation}.");
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

            //add the current term to the output
            output = (Expression)output.Add(currentTerm);

            //return the output
            return output;
        }

        private static string[] SubArray(string[] strs, int start, int end)
        {
            int length = end - start + 1;

            string[] output = new string[length];

            Array.Copy(strs, start, output, 0, length);

            return output;
        }


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

        private static bool IsOperator(char c)
        {
            return operators.Contains(c);
        }

        private static bool IsOpeningBracket(char c)
        {
            int index = brackets.IndexOf(c);

            return index >= 0 && index % 2 == 0;
        }

        private static bool IsClosingBracket(char c)
        {
            int index = brackets.IndexOf(c);

            return index >= 0 && index % 2 == 1;
        }

        private static bool IsBracket(char c)
        {
            return brackets.Contains(c);
        }

        private static bool IsMatchingBrackets(char left, char right)
        {
            int index = brackets.IndexOf(left);

            //if the left bracket does not exist/is at an odd index, it is not a bracket, or not suppposed to be on the left
            if (index == -1 || index % 2 == 1) return false;

            //otherwise make sure the left matches the right
            return brackets[index + 1] == right;
        }
    }
}
