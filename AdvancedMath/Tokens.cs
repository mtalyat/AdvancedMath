using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    enum Tokens : byte
    {
        Empty = (byte)' ',
        Open_Parenthesis = (byte)'(',
        Close_Parenthesis = (byte)')',
        Add_Operator = (byte)'+',
        Subtract_Operator = (byte)'-',
        Multiply_Operator = (byte)'*',
        Divide_Operator = (byte)'/',
        Power_Operator = (byte)'^',
        Factorial_Operator = (byte)'!',
        Derivative_Operator = (byte)'\'',
        Sub = (byte)'_',
    }

    static class TokensExtensions
    {
        public static char ToChar(this Tokens token)
        {
            return (char)token;
        }
    }
}
