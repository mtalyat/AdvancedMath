using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AdvancedMath
{
    /// <summary>
    /// Operators are Functions that print dynamically based on the Operator.
    /// For instance, instead of printing "mod(x, 5)", it can be printed as "x % 5".
    /// </summary>
    public class Operator : Function
    {
        #region Common Operators

        /// <summary>
        /// Shorthand to create a modulus Operator.
        /// </summary>
        public static Operator Modulus => new Operator(Functions.GetMethodInfo("Modulus"), "{0} % {1}");

        /// <summary>
        /// Shorthand to create a factorial Operator.
        /// </summary>
        public static Operator Factorial => new Operator(Functions.GetMethodInfo("Factorial"), "!{0}");

        #endregion

        /// <summary>
        /// The format to be used when converting to a string.
        /// </summary>
        private string format;

        #region Constructors

        /// <summary>
        /// Creates a new Operator with the given method and ToString format.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="format"></param>
        public Operator(MethodInfo method, string format) : this(method, format, new Token[0]) { }

        /// <summary>
        /// Creates a new Operator with the given method, ToString format and arguments.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public Operator(MethodInfo method, string format, params Token[] args) : base(method, args)
        {
            this.format = format;
        }

        #endregion

        public override Token Clone()
        {
            return new Operator(methodInfo, format, arguments.ToArray());
        }

        public override string ToString()
        {
            //print the function based on the given format
            return string.Format(format, arguments.ToArray());
        }
    }
}
