using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    /// <summary>
    /// Variables are values that are not currently known, and may be known in the future.
    /// </summary>
    public class Variable : Operand
    {
        #region Common Variables

        /// <summary>
        /// Shorthand to create a new Variable with the symbol 'a'.
        /// </summary>
        public static Variable A => new Variable('a');

        /// <summary>
        /// Shorthand to create a new Variable with the symbol 'b'.
        /// </summary>
        public static Variable B => new Variable('b');

        /// <summary>
        /// Shorthand to create a new Variable with the symbol 'c'.
        /// </summary>
        public static Variable C => new Variable('c');


        /// <summary>
        /// Shorthand to create a new Variable with the symbol 'x'.
        /// </summary>
        public static Variable X => new Variable('x');

        /// <summary>
        /// Shorthand to create a new Variable with the symbol 'y'.
        /// </summary>
        public static Variable Y => new Variable('y');

        /// <summary>
        /// Shorthand to create a new Variable with the symbol 'z'.
        /// </summary>
        public static Variable Z => new Variable('z');

        #endregion

        /// <summary>
        /// The symbol that represents this Variable.
        /// </summary>
        public char Symbol { get; private set; }

        /// <summary>
        /// The sub of this Variable. If the sub is equal to the default (0), it is not displayed.
        /// </summary>
        public uint Sub { get; private set; }

        public override bool IsConstant => false;

        public override bool HasConstantOrVariable => true;

        public override bool IsNumber => false;

        public override bool IsOne => false;

        public override bool IsZero => false;

        #region Constructors

        /// <summary>
        /// Creates a new Variable with the given symbol.
        /// </summary>
        /// <param name="symbol"></param>
        public Variable(char symbol) : this(symbol, 0) { }

        /// <summary>
        /// Creates a new Variable with the given symbol and sub.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="sub"></param>
        public Variable(char symbol, uint sub)
        {
            Symbol = symbol;
            Sub = sub;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="sub"></param>
        /// <param name="negative"></param>
        protected Variable(char symbol, uint sub, bool negative)
        {
            Symbol = symbol;
            Sub = sub;
            isNegative = negative;
        }

        #endregion

        #region Solving

        public override Token Evaluate(Scope scope)
        {
            //just return the corresponding value from the scope
            Operand operand = scope.Get(this);

            //apply negative
            operand.SetNegative(IsNegative);

            return operand;
        }

        public override Token Simplify()
        {
            //variables cannot be simplified
            return Clone();
        }

        #endregion

        #region Operations

        public override bool Equals(object obj)
        {
            return obj is Variable v && v.Symbol == Symbol && v.Sub == Sub;
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode() ^ Sub.GetHashCode();
        }

        public override Token Add(Token token)
        {
            //if it is a term, put the variable in a term, in an expression
            if (token is Term t)
            {
                return new Expression(t, new Term((Variable)Clone()));
            }
            else if (token is Expression expr)
            {
                //otherwise if it is already an expression, add to the expression
                return expr.Add((Variable)Clone());
            }
            else
            {
                //and at last, just make a new expression and add to it
                return new Expression(new Term((Element)token), new Term((Variable)Clone()));
            }
        }

        public override Token Multiply(Token token)
        {
            //if it is a term, multiply into it
            if (token is Term t)
            {
                return t.Multiply((Variable)Clone());
            }
            else if (token is Variable v && v.Equals(this))
            {
                //if it is another variable of the same type
                //put it in a term
                return new Term(Number.One, (Variable)Clone(), Number.Two).Simplify();
            }
            else if (token is Expression e)
            {
                //if it is an expression, multiply it into the expression
                return e.Multiply((Variable)Clone());
            }
            else if (token is Number n && n == -1)
            {
                //if it is -1, just flip the negative value
                Variable clone = (Variable)Clone();
                clone.isNegative = !isNegative;
                return clone;
            }
            else
            {
                //otherwise, just make a new term including both
                return new Term(new Element[] { (Element)token, (Element)Clone() });
            }
        }

        public override Number ToNumber()
        {
            //variables cannot be turned into a number without a scope
            return Number.NaN;
        }

        #endregion

        public override Token Clone()
        {
            return new Variable(Symbol, Sub, IsNegative);
        }

        public override string ToString()
        {
            if (Sub > 0)
            {
                return $"{(IsNegative ? "-" : "")}{Symbol}_{Sub}";
            }
            else
            {
                //if the sub is 0, no point in writing it
                return (IsNegative ? "-" : "") + Symbol.ToString();
            }
        }
    }
}
