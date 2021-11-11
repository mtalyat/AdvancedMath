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
    public abstract class Token
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
        /// Ex. "5" is a number, but "e" or "3/2" are not numbers.
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

        /// <summary>
        /// Evaluates the Token to the best ability it can, given the scope, and returns it.
        /// Evaluate will not necessarily retain the same object type.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns>An evaluated version of this Token.</returns>
        public abstract Token Evaluate(Scope scope);

        /// <summary>
        /// Returns a copy of this Token, simplified.
        /// Simplify will retain the same object type.
        /// </summary>
        /// <returns>A simplified copy of this Token.</returns>
        public abstract Token Simplify();

        /// <summary>
        /// Reduces this Token to another type, if able.
        /// Ex. A Term with just a Variable in it will return the Variable.
        /// </summary>
        /// <returns></returns>
        public abstract Token Reduce();

        /// <summary>
        /// Expands the Token, if able. 
        /// The Token will retain the same object type.
        /// </summary>
        /// <returns></returns>
        public abstract Token Expand();

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
        public virtual string ToStringParentheses(bool wrapInParenthesis)
        {
            StringBuilder sb = new StringBuilder();

            if (wrapInParenthesis) sb.Append(Tokens.Open_Parenthesis.ToChar());

            sb.Append(ToString());

            if (wrapInParenthesis) sb.Append(Tokens.Close_Parenthesis.ToChar());

            return sb.ToString();
        }
    }
}
