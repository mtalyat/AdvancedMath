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
        public const char MULTIPLICATION = '*';
        public const char DIVISION = '/';
        public const char POWER = '^';
        public const char FACTORIAL = '!';
        public const char MODULUS = '%';

        public const char SEPARATOR = ',';

        public const char SUB = '_';
    }
}
