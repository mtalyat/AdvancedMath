using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    public partial class Term
    {
        /// <summary>
        /// TermToken are Tokens that can only belong inside of Terms. 
        /// Each Element inside of a Term can have an exponent (Element).
        /// 
        /// Handles the power operations.
        /// </summary>
        public class TermToken : Token
        {
            #region Common TermElements

            /// <summary>
            /// Shorthand for a new TermElement that is equal to 1.
            /// </summary>
            public static TermToken One => new TermToken(Number.One, Number.Zero);

            #endregion

            /// <summary>
            /// The Token inside of this TermElement.
            /// </summary>
            public Token Token { get; private set; }

            /// <summary>
            /// The exponent to enact upon the Token.
            /// </summary>
            public Token Exponent { get; private set; }

            public override bool IsConstant => Token.IsConstant && Exponent.IsConstant;

            public override bool HasConstantOrVariable => Token.HasConstantOrVariable || Exponent.HasConstantOrVariable;

            public override bool IsNumber => Exponent.IsOne && Token.IsNumber;

            public override bool IsOne => Exponent.IsZero || Token.IsOne;

            public override bool IsZero => !Exponent.IsZero && Token.IsZero;

            //if the element is negative, and the exponent is an odd number, it will be a negative number
            public override bool IsNegative => Token.IsNegative && (Exponent.IsNumber && Exponent.ToNumber().IsWholeNumber && Exponent.ToNumber() % 2 == 1);

            #region Constructors

            /// <summary>
            /// Creates a new TermToken with the given element and exponent.
            /// </summary>
            /// <param name="token"></param>
            /// <param name="expo"></param>
            public TermToken(Token token, Token expo)
            {
                Token = token;
                Exponent = expo;
            }

            #endregion

            #region General

            public override Token Expand()
            {
                //we can only expand if the exponent is a number, and the element is an expression
                if (Exponent.IsZero || !Exponent.IsNumber || !(Token is Expression)) return Clone();

                Expression output = (Expression)Token;

                for (int i = 1; i < Exponent.ToNumber(); i++)
                {
                    output = output.FOIL((Expression)Token.Clone());
                }

                return new TermToken(output, Number.One);
            }

            #endregion

            #region Solving

            public override Token Simplify()
            {
                //does not matter what the element is if the exponent is 0
                if (Exponent.IsZero) return One;

                //simplify the values
                TermToken clone = (TermToken)Clone();
                clone.Token = clone.Token.Simplify();
                clone.Exponent = clone.Exponent.Simplify();

                if (clone.Token.IsNumber && clone.Exponent.IsConstant)
                {
                    Number n = clone.Token.ToNumber();

                    Number pow = Math.Pow(n, clone.Exponent.ToNumber());

                    //if the element number is a whole number, it must stay a whole number
                    if (!n.IsWholeNumber || pow.IsWholeNumber)
                    {
                        return new TermToken(pow, Number.One);
                    }
                }

                //otherwise just simplify the element and exponent

                return clone;
            }

            public override Token Evaluate(Scope scope)
            {
                Token expo = Exponent.Evaluate(scope);
                if (expo.IsZero) return Number.One;
                if (expo.IsOne) return Token.Evaluate(scope);

                Token ele = Token.Evaluate(scope);

                //if here, then must evaluate the element multiplied by itself, if there are no constants or variables left
                if (ele.HasConstantOrVariable || expo.HasConstantOrVariable) return new TermToken((Element)(ele is Term t ? new Expression(t) : ele), (Element)(expo is Term s ? new Expression(s) : expo));

                //no constants or variables, so we can actually evaluate it
                if(ele.IsNumber)
                {
                    return new Number(Math.Pow(ele.ToNumber(), expo.ToNumber()));
                } else
                {
                    //if not a number, multiply it by itself several times
                    Token output = Number.One;

                    for (int i = 0; i < expo.ToNumber(); i++)
                    {
                        output = output.Multiply(ele);
                    }

                    return output;
                }
            }

            public override Token Reduce()
            {
                //reduce if exponent == 1
                if(Exponent.IsZero)
                {
                    return Number.One;
                } else if (Exponent.IsOne)
                {
                    return Token.Reduce();
                }
                else
                {
                    return Clone();
                }
            }

            #endregion

            #region Operations

            public override Token Add(Token token)
            {
                //if the exponents match, add the tokens
                if(token is TermToken t && Exponent == t.Exponent)
                {
                    return new TermToken(Token.Add(token), Exponent);
                } else
                {
                    return new Expression(Clone(), token);
                }
            }

            public override Token Multiply(Token token)
            {
                //if the other token is also a TermToken, combine if tokens match
                if(token is TermToken t && Token == t.Token)
                {
                    return new TermToken(Token, Exponent.Add(t.Exponent));
                } else if (Exponent.IsZero)
                {
                    return token;
                } else if (Exponent.IsOne)
                {
                    return Token.Multiply(token);
                } else
                {
                    //if all else fails, just throw it in a term
                    //return new Term(new Token[] { Clone(), token });
                    return new Term(Clone()).Multiply(token);
                }
            }

            public override bool Equals(object obj)
            {
                return obj is TermToken te && Token.Equals(te.Token) && Exponent.Equals(te.Exponent);
            }

            public override int GetHashCode()
            {
                return Token.GetHashCode() ^ Exponent.GetHashCode();
            }

            public override Number ToNumber()
            {
                return Token.ToNumber();
            }

            #endregion

            public override Token Clone()
            {
                return new TermToken(Token.Clone(), Exponent.Clone());
            }

            public override string ToString()
            {
                if (Exponent.IsZero) return "1";

                //always write the element
                //wrap in parentheses if Element is an Expression
                StringBuilder sb = new StringBuilder(Token.ToStringParentheses(Token is Expression));

                //write the exponent, if it is not one
                if (!(Exponent.Simplify() is Number n) || n != 1)
                {
                    sb.Append(Tokens.POWER);
                    sb.Append(Exponent.ToStringParentheses(Exponent is Expression));
                }

                return sb.ToString();
            }
        }
    }
}
