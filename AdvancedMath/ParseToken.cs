using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    public static partial class Parse
    {
        class ParseToken
        {
            private string token;

            public bool IsOperator => IsOperator(token[0]);

            public bool IsFunction => Functions.GetFunction(token) != null;

            public bool IsOpenParenthesis => IsToken(Tokens.OPEN_PARENTHESIS);

            public bool IsCloseParenthesis => IsToken(Tokens.CLOSE_PARENTHESIS);

            public bool IsOperand => IsVariable(token) || CheckIfNumber();

            public int Precedence => GetPrecedence();

            public ParseToken(string str)
            {
                token = str;
            }

            private bool CheckIfNumber()
            {
                double d;

                return double.TryParse(token, out d);
            }

            private int GetPrecedence()
            {
                if (token.Length > 1) return 0;

                return Tokens.GetOperatorPrecedence(token[0]);
            }

            public bool IsToken(char c)
            {
                if (token.Length > 1) return false;

                return token[0] == c;
            }

            public Operand ToOperand()
            {
                double d;
                Constant c;

                if (double.TryParse(token, out d))
                {
                    return new Number(d);
                }
                else if (Constant.TryGetConstant(token, out c))
                {
                    return c;
                }
                else if (IsVariable(token))
                {
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
                    return null;
                }
            }

            public Function ToFunction()
            {
                Function f;

                Functions.TryGetFunction(token, out f);

                return f;
            }

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