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
    internal static class Symbols
    {
        public const char EMPTY = ' ';

        public const char OPEN_PARENTHESIS = '(';
        public const char CLOSE_PARENTHESIS = ')';

        public const char EQUALS = '=';
        public const char NOT_EQUALS = '≠';

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

        /// <summary>
        /// Determines if the given char is an operator.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the amount of operands the given operator will need.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>The amount of operands if c is an operator, otherwise 0.</returns>
        public static int OperatorTokenCount(char c)
        {
            switch (c)
            {
                case NEGATION:
                case FACTORIAL:
                    return 1;
                case ADDITION:
                case SUBTRACTION:
                case MULTIPLICATION:
                case IMPLICIT_MULTIPLICATION:
                case DIVISION:
                case POWER:
                case MODULUS:
                    return 2;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the precedence of the given char, if it is an operator.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>The precedence of the operator, otherwise 0.</returns>
        public static int GetOperatorPrecedence(char c)
        {
            switch (c)
            {
                case Symbols.OPEN_PARENTHESIS:
                case Symbols.CLOSE_PARENTHESIS:
                    return 7;
                case Symbols.SEPARATOR:
                    return 6;
                case Symbols.NEGATION:
                    return 5;
                case Symbols.POWER:
                    return 4;
                case Symbols.IMPLICIT_MULTIPLICATION:
                case Symbols.MULTIPLICATION:
                case Symbols.MODULUS:
                case Symbols.DIVISION:
                    return 3;
                case Symbols.SUBTRACTION:
                    return 2;
                case Symbols.ADDITION:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}
