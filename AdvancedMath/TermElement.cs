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
        /// TermElements are Elements that can only belong inside of Terms. 
        /// Each Element inside of a Term can have an exponent (Element).
        /// 
        /// Handles the power operations.
        /// </summary>
        public class TermElement : Element
        {
            #region Common TermElements

            /// <summary>
            /// Shorthand for a new TermElement that is equal to 1.
            /// </summary>
            public static TermElement One => new TermElement(Number.One, Number.Zero);

            #endregion

            /// <summary>
            /// The Element inside of this TermElement.
            /// </summary>
            public Element Element { get; private set; }

            /// <summary>
            /// The exponent to enact upon the Element.
            /// </summary>
            public Element Exponent { get; private set; }

            public override bool IsConstant => Element.IsConstant && Exponent.IsConstant;

            public override bool HasConstantOrVariable => Element.HasConstantOrVariable || Exponent.HasConstantOrVariable;

            public override bool IsNumber => Exponent.IsOne && Element.IsNumber;

            public override bool IsOne => Exponent.IsZero || Element.IsOne;

            public override bool IsZero => !Exponent.IsZero && Element.IsZero;

            //if the element is negative, and the exponent is an odd number, it will be a negative number
            public override bool IsNegative => Element.IsNegative && (Exponent.IsNumber && Exponent.ToNumber().IsWholeNumber && Exponent.ToNumber() % 2 == 1);

            #region Constructors

            /// <summary>
            /// Creates a new TermElement with the given element and exponent.
            /// </summary>
            /// <param name="element"></param>
            /// <param name="expo"></param>
            public TermElement(Element element, Element expo)
            {
                Element = element;
                Exponent = expo;
            }

            #endregion

            #region General

            public override Token Expand()
            {
                //we can only expand if the exponent is a number, and the element is an expression
                if (Exponent.IsZero || !Exponent.IsNumber || !(Element is Expression)) return Clone();

                Expression output = (Expression)Element;

                for (int i = 1; i < Exponent.ToNumber(); i++)
                {
                    output = output.FOIL((Expression)Element.Clone());
                }

                return new TermElement(output, Number.One);
            }

            #endregion

            #region Solving

            public override Token Simplify()
            {
                //does not matter what the element is if the exponent is 0
                if (Exponent.IsZero) return One;

                //simplify the values
                TermElement clone = (TermElement)Clone();
                clone.Element = (Element)clone.Element.Simplify();
                clone.Exponent = (Element)clone.Exponent.Simplify();

                if (clone.Element.IsNumber && clone.Exponent.IsConstant)
                {
                    Number n = clone.Element.ToNumber();

                    Number pow = Math.Pow(n, clone.Exponent.ToNumber());

                    //if the element number is a whole number, it must stay a whole number
                    if (!n.IsWholeNumber || pow.IsWholeNumber)
                    {
                        return new TermElement(pow, Number.One);
                    }
                }

                //otherwise just simplify the element and exponent

                return clone;
            }

            public override Token Evaluate(Scope scope)
            {
                Token expo = Exponent.Evaluate(scope);
                if (expo.IsZero) return Number.One;
                if (expo.IsOne) return Element.Evaluate(scope);

                Token ele = Element.Evaluate(scope);

                //if here, then must evaluate the element multiplied by itself, if there are no constants or variables left
                if (ele.HasConstantOrVariable || expo.HasConstantOrVariable) return new TermElement((Element)(ele is Term t ? new Expression(t) : ele), (Element)(expo is Term s ? new Expression(s) : expo));

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
                    return Element.Reduce();
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
                //TODO: If Element and Exponent match, put in a term and set the coefficient to two
                //If already a term and they match, add 1 to the coefficient
                //Otherwise, put inside of an expression
                throw new NotImplementedException();
            }

            public override Token Multiply(Token token)
            {
                //TODO: put both tokens in a new Term, and simplify
                throw new NotImplementedException();
            }

            public override bool Equals(object obj)
            {
                return obj is TermElement te && Element.Equals(te.Element) && Exponent.Equals(te.Exponent);
            }

            public override int GetHashCode()
            {
                return Element.GetHashCode() ^ Exponent.GetHashCode();
            }

            public override Number ToNumber()
            {
                return Element.ToNumber();
            }

            #endregion

            public override Token Clone()
            {
                return new TermElement((Element)Element.Clone(), (Element)Exponent.Clone());
            }

            public override string ToString()
            {
                if (Exponent.IsZero) return "1";

                //always write the element
                //wrap in parentheses if Element is an Expression
                StringBuilder sb = new StringBuilder(Element.ToStringParentheses(Element is Expression));

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
