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
    public class ParsingException : Exception
    {
        private readonly string errorToken;

        public override string Message
        {
            get
            {
                if (errorToken == "")
                {
                    return base.Message;
                }
                else
                {
                    return $"{base.Message} ({errorToken})";
                }
            }
        }

        public ParsingException() : base()
        {
            errorToken = "";
        }

        public ParsingException(string message) : base(message)
        {
            errorToken = "";
        }

        public ParsingException(string message, object token) : base(message)
        {
            errorToken = token.ToString();
        }
    }
}
