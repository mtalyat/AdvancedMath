using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    /// <summary>
    /// A number is a constant value that can be operated on, such as 5, -3, or 4.24.
    /// All numbers are represented in a double floating point format, and can be used interchangably with such.
    /// </summary>
    public class Number : Operand
    {
        #region Common Numbers

        /// <summary>
        /// Shorthand to create a new Number with the value 1.
        /// </summary>
        public static Number One => new Number(1);

        /// <summary>
        /// Shorthand to create a new Number with the value -1.
        /// </summary>
        public static Number NegativeOne => new Number(-1);

        /// <summary>
        /// Shorthand to create a new Number with the value 0.
        /// </summary>
        public static Number Zero => new Number(0);

        /// <summary>
        /// Shorthand to create a new Number with the value 2.
        /// </summary>
        public static Number Two => new Number(2);

        /// <summary>
        /// Shorthand to create a new Number with the value 10.
        /// </summary>
        public static Number Ten => new Number(10);

        #endregion

        /// <summary>
        /// The value the Number holds.
        /// </summary>
        public double Value { get; private set; }

        /// <summary>
        /// This Number as an integer. If the Number is not a whole number, it will be rounded to the nearest whole number.
        /// </summary>
        public int Integer => (int)Math.Round(Value);

        /// <summary>
        /// Is true when the Value corresponding to this Number is a whole number.
        /// </summary>
        public bool IsWholeNumber => Math.Round(Value) == Value;

        public override bool IsConstant => true;
        
        /// <summary>
        /// Is true if there is a corresponding symbol for this Number.
        /// </summary>
        public virtual bool HasSymbol => false;

        public override bool IsNumber => true;

        // so you can interchangably use double or Number, and they act as the same thing.
        public static implicit operator double(Number n) => n.Value;
        public static implicit operator Number(double d) => new Number(d);

        #region Constructors

        /// <summary>
        /// Creates a new Number with the default value, 0.
        /// </summary>
        public Number() : this(0) { }

        /// <summary>
        /// Creates a new Number with the corresponding value, val.
        /// </summary>
        /// <param name="val"></param>
        public Number(double val)
        {
            Value = val;
        }

        #endregion

        #region Solving

        public override Token Simplify()
        {
            //the Number is already as simplified as it can get, so just return a copy
            return Clone();
        }

        public override Token Evaluate(Scope scope)
        {
            //the Number is already as simplified and as evaluated as it can get, so just return a copy
            return Clone();
        }

        #endregion

        #region Operations

        public override bool Equals(object obj)
        {
            return obj is Number n && n.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override Token Add(Token token)
        {
            //if the token is a number, and not a constant, you can just add them together
            if((token is Number n) && !n.HasSymbol)
            {
                return new Number(Value + n);
            } else if (token is Expression e)
            {
                //if the token is an expressoin, just add itself to the expression
                return e.Add(Clone());
            } else
            {
                //if the term is anything else, this must go into an expression
                return new Expression(Clone(), token);
            }
        }

        public override Token Multiply(Token token)
        {
            //if the other token is also a number, and not a constant, just multiply them together
            if ((token is Number n) && !n.HasSymbol)
            {
                return new Number(Value * n);
            }
            else if (token is Term t)
            {
                //if the token is a term, just add this number to the numerator of the term
                return t.Add(Clone());
            }
            else
            {
                //if the term is anything else, this must go into a term
                return new Term(new Element[] { (Number)Clone(), (Element)token });
            }
        }

        public override Number ToNumber()
        {
            return (Number)Clone();
        }

        #endregion

        #region Operators

        /*
         * Operators added so that when adding two Numbers, they stay as a Number type instead of being turned into a double type.
         */

        public static Number operator +(Number left, Number right)
        {
            return new Number(left.Value + right.Value);
        }

        public static Number operator -(Number left, Number right)
        {
            return new Number(left.Value - right.Value);
        }

        public static Number operator *(Number left, Number right)
        {
            return new Number(left.Value * right.Value);
        }

        public static Number operator /(Number left, Number right)
        {
            return new Number(left.Value / right.Value);
        }

        public static Number operator %(Number left, Number right)
        {
            return new Number(left.Value % right.Value);
        }

        public static bool operator ==(Number left, Number right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Number left, Number right)
        {
            return !left.Equals(right);
        }

        #endregion

        public override Token Clone()
        {
            return new Number(Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
