using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    public static partial class Parse
    {
        /*
         * In the future...
         * 
         * [ ] used for matrices
         * { } used for Laplace transforms
         * < > used for vectors
         * ( ) used for parenthesis
         */

        /*
         * (New) Parsing Strategy:
         * 
         * Read string. Break up into tokens.
         * Insert # operators where needed.
         * "4x^2 + 3/4x - -2" --> [4, •, x, ^, 2, +, 3, /, 4, •, x, +, 2]
         * "4min(2x, 5y) + 3" --> [4, •, min, (, 2, •, x, ,, 5, •, y, ), +, 3]
         * 
         * Sort from infix notation to postfix notation
         * [4, •, x, ^, 2, +, 3, /, 4, •, x, +, 2] --> [4, x, •, 2, ^, 3, +, 3, 4, x, •, /, 2, +]
         * [4, •, min, (, 2, •, x, ,, 5, •, y, ), +, 3] --> [4, 2, x, •, 5, y, •, min, •, 3, +]
         * 
         * Evaluate into an expression
         * [4, x, •, 2, ^, 3, +, 3, 4, x, •, /, 2, +] --> 4x^2 + 3/4x + 2
         * [4, 2, x, •, 5, y, •, min, •, 3, +] --> 4min(2x, 5y) + 3
         */

        private static string brackets = "()";

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

        #region Parsing

        /// <summary>
        /// Parses a string into a Token.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Token ParseString(string str)
        {
            //split the string into stringified tokens
            //turn those tokens into managable ParseTokens
            //shuffle the parse tokens from infix notation to postfix notation
            //evaluate the postfix notation into a singular token
            return PostfixToToken(InfixToPostfix(StringTokensToParseTokens(SplitString(str)))).Reduce();
        }

        #endregion

        #region Helper Methods

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
            char lastLastC = ' ';

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
                //the last character was an operator or a bracket, and this character is a letter or digit AND
                // NOT (the last LAST character was an operator, the last character was a subtraction, and this character is a digit)
                if (IsBracket(c) || IsOperator(c) ||
                    (char.IsLetter(c) && current.Length > 0 && char.IsDigit(current[current.Length - 1])) ||
                    ((IsOperator(lastC) || IsBracket(lastC)) && char.IsLetterOrDigit(c)))
                {
                    //if it is a bracket, put whatever has been accumulating into a token
                    //if it is an operator, put current into the output and keep going

                    //if this is a letter, and the beginning of current is '-' or a number, then this is a separate thing

                    //if a number next to a letter, we need to insert a multiplier in there
                    //same with if the last one was a letter/digit and this one is an opening bracket, we are multiplying
                    char multiply = ' ';
                    if((char.IsLetter(c) && current.Length > 0 && char.IsDigit(current[current.Length - 1])) ||
                        (char.IsLetterOrDigit(lastC) && IsOpeningBracket(c)))
                    {
                        multiply = Tokens.IMPLICIT_MULTIPLICATION;
                    }

                    if(IsClosingBracket(lastC) && IsOpeningBracket(c))
                    {
                        multiply = Tokens.MULTIPLICATION;
                    }

                    //only add if there is something there
                    if (current.Length > 0)
                    {
                        output.Add(current.ToString());
                        current.Clear();

                        if (multiply != ' ')
                        {
                            //implicit, so it has more precedence than a ^ operator
                            output.Add(multiply.ToString());
                        }
                    }
                }

                //everything else can just be added regardless of what is in front

                //if it is a letter, it could be a function name or a variable, so we just add it anyways
                //if it is a number, and the first char of current is a letter, this is part of a function name
                //if it is a number, and the first char of current is '-' or a number, this is part of a number
                //either way, add it

                //swap out with negation if needed
                if(c == Tokens.SUBTRACTION && (lastC == ' ' || IsOperator(lastC)))
                {
                    if(lastC == ' ')
                    {
                        output.Add("-1");
                        current.Append(Tokens.IMPLICIT_MULTIPLICATION.ToString());
                    } else
                    {
                        current.Append(Tokens.NEGATION);
                    }
                } else
                {
                    current.Append(c);
                }

                lastLastC = lastC;
                lastC = c;
            }

            //add the remaining current to the output
            output.Add(current.ToString());

            //lastly, return the split up string
            return output.ToArray();
        }

        /// <summary>
        /// Puts all string tokens into ParseTokens, which make determining what each string token is easier.
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        private static ParseToken[] StringTokensToParseTokens(string[] strs)
        {
            return strs.Select(s => new ParseToken(s)).ToArray();
        }

        /// <summary>
        /// Converts an array of ParseTokens from Infix notation to Postfix notation.
        /// </summary>
        /// <param name="inTokens"></param>
        /// <returns></returns>
        private static ParseToken[] InfixToPostfix(ParseToken[] inTokens)
        {
            Queue<ParseToken> tokens = new Queue<ParseToken>(inTokens);

            Stack<ParseToken> outputs = new Stack<ParseToken>();
            Stack<ParseToken> operators = new Stack<ParseToken>();


            while (tokens.Count > 0)
            {
                ParseToken token = tokens.Dequeue();

                if (token.IsOperand)
                {
                    outputs.Push(token);
                }
                else if (token.IsFunction)
                {
                    operators.Push(token);

                    //the very next token will be an implicit operator, so we want to ignore that one
                    tokens.Dequeue();
                }
                else if (token.IsOperator)
                {
                    if (operators.Any())
                    {
                        ParseToken op = operators.Peek();

                        int presDiff = op.Precedence - token.Precedence;

                        while (op.IsOperator && (presDiff > 0 || (presDiff == 0 && token.Precedence == 3)) && !op.IsOpenParenthesis)
                        {
                            outputs.Push(operators.Pop());

                            if (!operators.Any())
                            {
                                //if no more, break out of the loop
                                break;
                            }
                            else
                            {
                                //move on to the next one
                                op = operators.Peek();
                                presDiff = op.Precedence - token.Precedence;
                            }
                        }
                    }

                    operators.Push(token);
                }
                else if (token.IsToken(Tokens.SEPARATOR))
                {
                    if (operators.Any())
                    {
                        ParseToken op = operators.Peek();

                        while (op.IsOperator && !op.IsOpenParenthesis)
                        {
                            outputs.Push(operators.Pop());

                            if (!operators.Any())
                            {
                                //no more operators, leave the loop
                                break;
                            }
                            else
                            {
                                //move to the next
                                op = operators.Peek();
                            }
                        }
                    }
                }
                else if (token.IsOpenParenthesis)
                {
                    operators.Push(token);
                }
                else if (token.IsCloseParenthesis)
                {
                    //if close, add to output until open is found
                    while (operators.Any() && !operators.Peek().IsOpenParenthesis)
                    {
                        outputs.Push(operators.Pop());
                    }

                    //operators runs out, that means there is mismatched parenthesis
                    if (!operators.Any())
                    {
                        throw new ParsingException("Missing opening parenthesis.", ")");
                    }

                    //get rid of the parentheis
                    if (operators.Peek().IsOpenParenthesis)
                    {
                        operators.Pop();
                    }

                    //push the function itself, if there is one
                    if (operators.Any() && operators.Peek().IsFunction)
                    {
                        outputs.Push(operators.Pop());
                    }
                }
                else
                {
                    //something else
                    outputs.Push(token);
                }
            }

            if (tokens.Count == 0)
            {
                while (operators.Count > 0)
                {
                    //if an operator is a parenthesis, that means that there are mismatched parenthesis
                    if (operators.Peek().IsOpenParenthesis)
                    {
                        throw new ParsingException("Missing closing parenthesis.", "(");
                    }
                    else
                    {
                        outputs.Push(operators.Pop());
                    }
                }
            }

            return outputs.ToArray();
        }

        /// <summary>
        /// Takes an expression in postfix notation and returns the Token representation of it.
        /// </summary>
        /// <param name="inTokens"></param>
        /// <returns></returns>
        private static Token PostfixToToken(ParseToken[] inTokens)
        {
            if (inTokens.Length == 0) return Number.Zero;

            Stack<ParseToken> tokens = new Stack<ParseToken>(inTokens);

            Stack<Token> operands = new Stack<Token>();

            while (tokens.Any())
            {
                ParseToken token = tokens.Pop();

                Operand o = token.ToOperand();
                Function f = token.ToFunction();

                if (o != null)
                {
                    operands.Push(o);
                }
                else if (token.IsOperator)
                {
                    //if operator only uses one token, only grab one
                    if(Tokens.OperatorTokenCount(token.ToChar()) == 1)
                    {
                        Token t = operands.Pop();

                        operands.Push(EvaluateOperator(token.ToChar(), t, null));
                    } else
                    {
                        Token right = operands.Pop();
                        Token left = operands.Pop();

                        operands.Push(EvaluateOperator(token.ToChar(), left, right));
                    }
                }
                else if (f != null)
                {
                    //the next # of operands are for this function
                    int parameterCount = f.ParameterCount;

                    //get the operands in reverse order
                    Token[] args = new Token[parameterCount];

                    for (int i = parameterCount - 1; i >= 0; i--)
                    {
                        args[i] = operands.Pop();
                    }

                    //now add to function and add that to operands
                    f.AddArguments(args);

                    operands.Push(f);
                }
                else
                {
                    throw new ParsingException("Unknown token when converting from string to Token.", token);
                }
            }

            if (operands.Count > 1)
            {
                throw new ParsingException($"Parsing incomplete. Too many operands. Overflow count = {operands.Count}.");
            }

            return operands.Pop();
        }

        /// <summary>
        /// Evaluates two Tokens based on the operator.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private static Token EvaluateOperator(char op, Token left, Token right)
        {
            switch (op)
            {
                case Tokens.IMPLICIT_MULTIPLICATION:
                case Tokens.MULTIPLICATION:
                    return left.Multiply(right);
                case Tokens.POWER:
                    return new Term.TermToken(left, right);
                case Tokens.MODULUS:
                    Function mod = Operator.Modulus;
                    mod.AddArguments(new Token[] { left, right });
                    return mod;
                case Tokens.DIVISION:
                    return left.Multiply(Term.CreateFraction(Number.One, right));
                case Tokens.ADDITION:
                    return left.Add(right);
                case Tokens.SUBTRACTION:
                    return left.Add(right.Multiply(Number.NegativeOne));
                case Tokens.NEGATION:
                    return left.Multiply(Number.NegativeOne);
                case Tokens.FACTORIAL:
                    Function fact = Operator.Factorial;
                    fact.AddArgument(left);
                    return fact;
                default:
                    throw new ParsingException("Unimplemented operator functionality.", op.ToString());
            }
        }

        #region Char Determination

        /// <summary>
        /// Determines if the given char is an operator.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsOperator(char c)
        {
            return Tokens.IsOperator(c);
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

        /// <summary>
        /// Determines if the given string can be parsed into a Variable.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool IsVariable(string str)
        {
            //variable if str matches "L" or "L_X...", where L is a letter and X is a digit
            return (str.Length == 1 && char.IsLetter(str[0])) || (str.Length >= 3 && str[1] == Tokens.SUB);
        }

        #endregion

        #endregion
    }
}
