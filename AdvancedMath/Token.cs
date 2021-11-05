using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    /// <summary>
    /// The base class for all equation related things.
    /// </summary>
    public abstract class Token
    {
        /// <summary>
        /// A Token is constant if either the token itself is constant, or if all values within the Token are constant.
        /// </summary>
        public abstract bool IsConstant { get; }

        /// <summary>
        /// A Token is a number if the Token can be represented as a singular number with no symbols.
        /// Ex. "5" is a number, but "e" or "3/2" are not numbers.
        /// </summary>
        public abstract bool IsNumber { get; }

        /// <summary>
        /// Returns a copy of this Token, simplified.
        /// </summary>
        /// <returns></returns>
        public abstract Token Simplify();

        /// <summary>
        /// Checks if this Token can be Evaluated into the given Number, without a Scope.
        /// Requires the Token to be constant, otherwise always returns false.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public bool IsValue(Number number)
        {
            //if it is not constant, it cannot be evaluated to a value
            if (!IsConstant) return false;

            Token t = Evaluate(Scope.Empty);//no scope is needed if it is constant

            return t is Number n && n == number;
        }

        public bool IsOne => IsValue(1);
        public bool IsZero => IsValue(0);

        /// <summary>
        /// Evaluates the Token to the best ability it can, given the scope, and returns it.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public abstract Token Evaluate(Scope scope);

        /// <summary>
        /// Returns a clone of this Token.
        /// </summary>
        /// <returns></returns>
        public abstract Token Clone();

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

        /// <summary>
        /// Converts this Token into a Number, if able.
        /// Use IsNumber before ToNumber, otherwise there may be unexpected results.
        /// </summary>
        /// <returns>The Number the Token is, or null if the conversion is not possible.</returns>
        public abstract Number ToNumber();

        /// <summary>
        /// Converts this Token to a string, with optional parenthesis around the Token.
        /// </summary>
        /// <param name="wrapInParenthesis">When true, the Token will be wrapped in parethesis, if necessary.</param>
        /// <returns></returns>
        public virtual string ToString(bool wrapInParenthesis)
        {
            StringBuilder sb = new StringBuilder();

            if (wrapInParenthesis) sb.Append(Tokens.Open_Parenthesis.ToChar());

            sb.Append(ToString());

            if (wrapInParenthesis) sb.Append(Tokens.Close_Parenthesis.ToChar());

            return sb.ToString();
        }
    }
}
