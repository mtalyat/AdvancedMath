using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    class InterpreterException : Exception
    {
        public InterpreterException(string message) : base(message)
        {

        }
    }
}
