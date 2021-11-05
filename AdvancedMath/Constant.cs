using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    /// <summary>
    /// A constant is a constant Number that is represented by a symbol, such as pi or e.
    /// </summary>
    public class Constant : Number
    {
        #region Common Constants

        /// <summary>
        /// Shorthand to create a new Constant with the value of 𝝅 (pi).
        /// </summary>
        public static Constant PI => new Constant("𝝅", Math.PI);

        /// <summary>
        /// Shorthand to create a new Constant with the value of e (Euler's number).
        /// </summary>
        public static Constant E => new Constant("e", Math.E);

        #endregion

        /// <summary>
        /// The symbol that corresponds with this Constant.
        /// </summary>
        public string Symbol { get; private set; }

        public override bool HasSymbol => true;

        // Returning false. It is a Number, but for most operations we want the Constant to not be mixed with other Numbers.
        public override bool IsNumber => false;

        #region Constructors

        /// <summary>
        /// Creates a new Constant with the given symbol, and given value.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="val"></param>
        public Constant(string symbol, double val) : base(val)
        {
            Symbol = symbol;
        }

        #endregion

        #region Operations

        public override Number ToNumber()
        {
            //ignore the symbol, just a number now
            return new Number(Value);
        }

        #endregion

        public override Token Clone()
        {
            return new Constant(Symbol, Value);
        }

        public override string ToString()
        {
            return Symbol;
        }
    }
}
