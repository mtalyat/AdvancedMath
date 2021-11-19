using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    /*
     * Some notes about Tokens:
     * 
     * Simplify will simplify tokens. It will not change their type, but it will reduce them. For instance, a fraction in a Term could go from 6/2 to 3.
     * 
     * Evaluate will replace variables with their respective values, and then simplify, except it will change the type, if able.
     * 
     * TODO: Solve will find the values for all unknown variables in the equation.
     * 
     * IsConstant will return true if all tokens are constant (Constant, Number, or made up of those two). (No variables)
     * IsNumber will return true if all tokens are numbers, but not Constants.
     * HasConstant will return true if the token contains a Constant at all.
     */

    /// <summary>
    /// The base class for all equation related things.
    /// </summary>
    public abstract class Token : IMathematical<Token>
    {
        /// <summary>
        /// A Token is constant if either the token itself is constant, or if all values within the Token are constant.
        /// </summary>
        public abstract bool IsConstant { get; }

        /// <summary>
        /// A Token has a Constant if there is a Constant or Variable stored anywhere within the Token, or if the Token is a Constant or Variable itself.
        /// </summary>
        public abstract bool HasConstantOrVariable { get; }

        /// <summary>
        /// A Token is a number if the Token can be represented as a singular number with no symbols.
        /// Ex. "5" or "2.25" are numbers, but "e" or "3/2" are not numbers.
        /// </summary>
        public abstract bool IsNumber { get; }

        /// <summary>
        /// True if this Token can easily be represented as 1. Will not evaluate the Token.
        /// </summary>
        public abstract bool IsOne { get; }

        /// <summary>
        /// True if this Token can easily be represented as 0. Will not evaluate the Token.
        /// </summary>
        public abstract bool IsZero { get; }

        /// <summary>
        /// True if the Token is negative.
        /// </summary>
        public abstract bool IsNegative { get; }

        public abstract Token Evaluate(Scope scope);

        public abstract Token Simplify();

        public abstract Token Reduce();

        public abstract Token Expand();

        public abstract Token Clone();

        #region Operations

        /// <summary>
        /// Adds another Token to this Token.
        /// Forces the addition. For instance, if you try to add 1/2 and 1/3, it will combine them into one Term instead of just putting them in an Expression.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract Token Add(Token token);

        /// <summary>
        /// Multiplies another Token with this token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract Token Multiply(Token token);

        #region Operators

        /// <summary>
        /// Adding operator. Adds the right Token to the left Token.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Token operator +(Token left, Token right)
        {
            return left.Add(right);
        }

        /// <summary>
        /// Subtracting operator. Subtracts the right Token from the left Token.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Token operator -(Token left, Token right)
        {
            return left.Add(-right);
        }

        /// <summary>
        /// Negation operator. Multiplies the Token by -1.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Token operator -(Token t)
        {
            return t.Multiply(Number.NegativeOne);
        }

        /// <summary>
        /// Multiplying operator. Multiplies the left Token by the right Token.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Token operator *(Token left, Token right)
        {
            return left.Multiply(right);
        }

        /// <summary>
        /// Dividing operator. Divides the left Token by the right Token.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Token operator /(Token left, Token right)
        {
            return left.Multiply(Term.CreateFraction(Number.One, right));
        }

        /// <summary>
        /// Modulus operator. Puts left Token and right Token into a Function that performs modulus.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Token operator %(Token left, Token right)
        {
            Function f = Operator.Modulus;
            f.AddArguments(new Token[] { left, right });
            return f;
        }

        /// <summary>
        /// Power operator. Raises left Token to the power of right Token.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Token operator ^(Token left, Token right)
        {
            return new Term.TermToken(left, right);
        }

        /// <summary>
        /// Factorial operator. Puts the Token into a Function that performs factorial.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Token operator !(Token t)
        {
            Function f = Operator.Factorial;
            f.AddArgument(t);
            return f;
        }

        #endregion

        #endregion

        /// <summary>
        /// Converts this Token into a Number, if able.
        /// Use IsNumber or IsConstant before ToNumber, otherwise there may be unexpected results.
        /// </summary>
        /// <returns>The Number the Token is, or Number.NaN if the conversion is not possible.</returns>
        public abstract Number ToNumber();

        /// <summary>
        /// Converts this Token to an Expression.
        /// </summary>
        /// <returns></returns>
        public virtual Expression ToExpression()
        {
            return new Expression(Clone());
        }

        /// <summary>
        /// Converts this Token to a Term.
        /// </summary>
        /// <returns></returns>
        public virtual Term ToTerm()
        {
            return new Term(Clone());
        }

        /// <summary>
        /// Converts this Token to a string, with optional parenthesis around the Token.
        /// </summary>
        /// <param name="wrapInParenthesis">When true, the Token will be wrapped in parethesis, if necessary.</param>
        /// <returns></returns>
        public virtual string ToStringParentheses(bool wrapInParenthesis)
        {
            StringBuilder sb = new StringBuilder();

            if (wrapInParenthesis) sb.Append(Tokens.OPEN_PARENTHESIS);

            sb.Append(ToString());

            if (wrapInParenthesis) sb.Append(Tokens.CLOSE_PARENTHESIS);

            return sb.ToString();
        }
    }
}
