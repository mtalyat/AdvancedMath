using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    public static partial class Parse
    {
        class ParsingToken
        {
            /// <summary>
            /// The string token inside of this ParseToken.
            /// </summary>
            private readonly string token;

            /// <summary>
            /// Determines if this ParseToken is an operator.
            /// </summary>
            public bool IsOperator => IsOperator(token[0]);

            /// <summary>
            /// Determines if this ParseToken is a function.
            /// </summary>
            public bool IsFunction => Functions.GetFunction(token) != null;

            /// <summary>
            /// Determines if this ParseToken is an opening parenthesis.
            /// </summary>
            public bool IsOpenParenthesis => IsToken(Tokens.OPEN_PARENTHESIS);

            /// <summary>
            /// Determines if this ParseToken is a closing parenthesis.
            /// </summary>
            public bool IsCloseParenthesis => IsToken(Tokens.CLOSE_PARENTHESIS);

            /// <summary>
            /// Determines if this ParseToken is an Operand.
            /// </summary>
            public bool IsOperand => ToOperand() != null;

            /// <summary>
            /// Gets the precedence of this ParseToken. If this ParseToken is not an operator, Precedence will be 0.
            /// </summary>
            public int Precedence => GetPrecedence();

            /// <summary>
            /// Creates a new ParseToken with the given string token.
            /// </summary>
            /// <param name="str"></param>
            public ParsingToken(string str)
            {
                token = str;
            }

            /// <summary>
            /// Gets the precedence of this ParseToken. If this ParseToken is not an operator, Precedence will be 0.            
            /// </summary>
            /// <returns></returns>
            private int GetPrecedence()
            {
                if (token.Length > 1) return 0;

                return Tokens.GetOperatorPrecedence(token[0]);
            }

            /// <summary>
            /// Determines if this ParseToken is equal to the given char token.
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public bool IsToken(char c)
            {
                if (token.Length > 1) return false;

                return token[0] == c;
            }

            /// <summary>
            /// Converts this ParseToken to an Operand, if able.
            /// </summary>
            /// <returns>The Operand representation of this ParseToken, or null if unable to represent as an Operand.</returns>
            public Operand ToOperand()
            {
                double d;
                Constant c;

                if (double.TryParse(token, out d))
                {
                    //this is a number
                    return new Number(d);
                }
                else if (Constant.TryGetConstant(token, out c))
                {
                    //this is a constant
                    return c;
                }
                else if (IsVariable(token))
                {
                    //this is a variable
                    string[] split = token.Split(Tokens.SUB);

                    if (split.Length == 1)
                    {
                        return new Variable(split[0][0]);
                    }
                    else
                    {
                        return new Variable(split[0][0], uint.Parse(split[1]));
                    }
                }
                else
                {
                    //something else
                    return null;
                }
            }

            /// <summary>
            /// Converts this ParseToken into a Function, if able.
            /// </summary>
            /// <returns>The Function representation of this ParseToken, or null if unable to represent as an Function.</returns>
            public Function ToFunction()
            {
                Function f;

                Functions.TryGetFunction(token, out f);

                return f;
            }

            /// <summary>
            /// Gets the first char in the string token, and returns it.
            /// </summary>
            /// <returns></returns>
            public char ToChar()
            {
                return token[0];
            }

            public override string ToString()
            {
                return token;
            }
        }
    }
}