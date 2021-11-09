using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    /// <summary>
    /// ParsingExceptions are thrown when there is an error parsing a string in the Parse class.
    /// </summary>
    class ParsingException : Exception
    {
        public ParsingException() : base()
        {

        }

        public ParsingException(string message) : base(message)
        {

        }
    }
}
