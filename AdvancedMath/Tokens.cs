using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    /// <summary>
    /// This class holds constants used throughout the program for displaying operators and such.
    /// Put in a class and made const so that they can be easily modified if needed.
    /// </summary>
    internal static class Tokens
    {
        public const char EMPTY = ' ';

        public const char OPEN_PARENTHESIS = '(';
        public const char CLOSE_PARENTHESIS = ')';

        public const char EQUALS = '=';

        public const char ADDITION = '+';
        public const char SUBTRACTION = '-';
        public const char NEGATION = '~';
        public const char MULTIPLICATION = '*';
        public const char IMPLICIT_MULTIPLICATION = '#';//•
        public const char DIVISION = '/';
        public const char POWER = '^';
        public const char FACTORIAL = '!';
        public const char MODULUS = '%';

        public const char SEPARATOR = ',';

        public const char SUB = '_';

        public static bool IsOperator(char c)
        {
            return
                c == ADDITION ||
                c == SUBTRACTION ||
                c == NEGATION ||
                c == MULTIPLICATION ||
                c == IMPLICIT_MULTIPLICATION ||
                c == DIVISION ||
                c == POWER ||
                c == FACTORIAL ||
                c == MODULUS;
        }

        public static int OperatorTokenCount(char c)
        {
            switch (c)
            {
                case NEGATION:
                case FACTORIAL:
                    return 1;
                default:
                    return 2;
            }
        }

        public static int GetOperatorPrecedence(char c)
        {
            switch (c)
            {
                case Tokens.OPEN_PARENTHESIS:
                case Tokens.CLOSE_PARENTHESIS:
                    return 7;
                case Tokens.SEPARATOR:
                    return 6;
                case Tokens.NEGATION:
                    return 5;
                case Tokens.POWER:
                    return 4;
                case Tokens.IMPLICIT_MULTIPLICATION:
                case Tokens.MULTIPLICATION:
                case Tokens.MODULUS:
                case Tokens.DIVISION:
                    return 3;
                case Tokens.SUBTRACTION:
                    return 2;
                case Tokens.ADDITION:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}
