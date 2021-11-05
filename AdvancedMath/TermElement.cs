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

            public override bool IsNumber => Exponent.IsOne && Element.IsNumber;

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

            #region Solving

            public override Token Simplify()
            {
                //does not matter what the element is if the exponent is 0
                if (Exponent.IsValue(0)) return TermElement.One;

                //simplify the values
                TermElement clone = (TermElement)Clone();
                clone.Element = (Element)clone.Element.Simplify();
                clone.Exponent = (Element)clone.Exponent.Simplify();

                if (clone.Element.IsNumber && clone.Exponent.IsNumber)
                {
                    return new TermElement(new Number(Math.Pow(clone.Element.ToNumber(), clone.Exponent.ToNumber())), Number.One);
                }
                else
                {
                    return new TermElement((Element)Element.Simplify(), (Element)Exponent.Simplify());
                }
            }

            public override Token Evaluate(Scope scope)
            {
                Token expo = Exponent.Evaluate(scope);
                if (expo.IsZero) return Number.One;
                if (expo.IsOne) return Element.Evaluate(scope);

                Token val = Element.Evaluate(scope);

                if (expo is Number e)
                {
                    if (val is Number v)
                    {
                        return new Number(Math.Pow(v, e));
                    }
                    else if (e.IsWholeNumber && e > 0)
                    {
                        Token output = Number.One;

                        for (int i = 0; i < e; i++)
                        {
                            output = output.Multiply(val);
                        }

                        return output;
                    }
                }

                return new TermElement((Element)expo, (Element)val);
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
                return Element.Clone() as Number;
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
                StringBuilder sb = new StringBuilder(Element.ToString(Element is Expression));

                //write the exponent, if it is not one
                if (!(Exponent.Simplify() is Number n) || n != 1)
                {
                    sb.Append(Tokens.Power_Operator.ToChar());
                    sb.Append(Exponent.ToString(Exponent is Expression));
                }

                return sb.ToString();
            }
        }
    }
}
