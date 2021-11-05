using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedMath
{
    /// <summary>
    /// Terms are what make up expressions.
    /// Terms combine fractions, and the parts that make up a term: a coefficient, variables and exponents.
    /// The term has 1 denominator, which is an Expression.
    /// The term can have any amount of numerators, which are Elements.
    /// Each Element in the Numerator also has an exponent, which is an Expression.
    /// 
    /// Handles the multiplication and division operations.
    /// </summary>
    public partial class Term : Token
    {
        #region Common Terms

        /// <summary>
        /// Shorthand to create a new Term that is equal to 1.
        /// </summary>
        public static Term One => new Term(Number.One);

        /// <summary>
        /// Shorthand to create a new Term that is equal to 0.
        /// </summary>
        public static Term Zero => new Term(Number.Zero);

        #endregion

        //the term is only constant if all elements and exponents are constant
        //we know the coefficients will be constant since they are numbers
        public override bool IsConstant => denominators.All(e => e.IsConstant) && numerators.All(e => e.IsConstant);

        /// <summary>
        /// True if the denominator is equal to 1.
        /// </summary>
        private bool IsDenominatorOne => coefficientDenominator.IsOne && denominators.All(d => d.IsOne);

        public override bool IsNumber => numerators.Count == 1 && numerators[0].IsOne && IsDenominatorOne;

        /// <summary>
        /// The coefficient for the numerator.
        /// </summary>
        private Number coefficientNumerator;

        /// <summary>
        /// The coefficient for the denominator.
        /// </summary>
        private Number coefficientDenominator;

        /// <summary>
        /// The list of Elements in the numerator.
        /// </summary>
        private List<TermElement> numerators;

        /// <summary>
        /// The list of Elements in the denominator.
        /// </summary>
        private List<TermElement> denominators;

        #region Constructors

        /// <summary>
        /// Creates a Term from any type of Token.
        /// </summary>
        /// <param name="token"></param>
        public Term(Token token)
        {
            if(token is Term t)
            {
                //copy values if it is another term
                coefficientNumerator = t.coefficientNumerator;
                coefficientDenominator = t.coefficientDenominator;
                numerators = t.numerators;
                denominators = t.denominators;
            } else
            {
                //otherwise, just put it in the numerator and do defaults
                coefficientNumerator = Number.One;
                coefficientDenominator = Number.One;

                numerators = new List<TermElement>();
                AddToNumerator(token is TermElement te ? te : new TermElement((Element)token, Number.One));

                denominators = new List<TermElement>();
                AddToDenominator(TermElement.One);
            }

            EnsureNomAndDenomAreNotEmpty();
        }

        /// <summary>
        /// Creates a new Term using the given Elements as the numerator.
        /// </summary>
        /// <param name="numerators"></param>
        public Term(Element[] numerators) : this(Number.One, numerators) { }

        /// <summary>
        /// Creates a new Term with the given coefficient, and the given Elements as the numerator.
        /// </summary>
        /// <param name="coefficient"></param>
        /// <param name="numerators"></param>
        public Term(Number coefficient, Element[] numerators) : this(coefficient, Number.One, numerators, null) { }

        /// <summary>
        /// Creates a new Term with the given coefficient, and an Element with its corresponding Exponent that will go in the numerator.
        /// </summary>
        /// <param name="coefficient"></param>
        /// <param name="element"></param>
        /// <param name="exponent"></param>
        public Term(Number coefficient, Element element, Element exponent) : this(coefficient, new Element[] { new TermElement(element, exponent) }) { }

        /// <summary>
        /// Creates a new Term with the given list of Elements for both the numerator, and denominator.
        /// </summary>
        /// <param name="numerators"></param>
        /// <param name="denominators"></param>
        public Term(Element[] numerators, Element[] denominators) : this(Number.One, Number.One, numerators, denominators) { }

        /// <summary>
        /// Creates a new Term with both the coefficient and Elements for both the nominator and denominator.
        /// </summary>
        /// <param name="coefficientNumerator"></param>
        /// <param name="coefficientDenominator"></param>
        /// <param name="numerators"></param>
        /// <param name="denominators"></param>
        public Term(Number coefficientNumerator, Number coefficientDenominator, Element[] numerators, Element[] denominators)
        {
            this.coefficientNumerator = coefficientNumerator;
            this.coefficientDenominator = coefficientDenominator;
            this.numerators = new List<TermElement>();
            this.denominators = new List<TermElement>();

            foreach (TermElement te in ConvertElementsToTermElements(numerators ?? new Element[0]))
                AddToNumerator(te);

            foreach (TermElement te in ConvertElementsToTermElements(denominators ?? new Element[0]))
                AddToDenominator(te);

            EnsureNomAndDenomAreNotEmpty();
        }

        #endregion

        #region Numerator/Denominator Management

        /// <summary>
        /// Adds a 1 to the numerator or denominator if either one is empty.
        /// </summary>
        private void EnsureNomAndDenomAreNotEmpty()
        {
            //make sure there is at least something
            if (!numerators.Any()) numerators.Add(TermElement.One);
            if (!denominators.Any()) denominators.Add(TermElement.One);
        }

        /// <summary>
        /// Converts the given list of Elements into TermElements. If the Element already is a TermElement, is uses that. 
        /// If not, it puts the Element inside of a TermElement.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        private TermElement[] ConvertElementsToTermElements(Element[] elements)
        {
            return elements.Select(e => e is TermElement te ? te : new TermElement(e, Number.One)).ToArray();
        }

        /// <summary>
        /// Adds the given TermElement to the given List of TermElements, or multiplies it into the given coefficient if the TermElement is a Number.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="list"></param>
        /// <param name="coefficient"></param>
        private void AddToList(TermElement e, List<TermElement> list, ref Number coefficient)
        {
            if (e.Exponent.IsZero) return;//if it is 1, it literally does not matter

            //if the value is a number, but not a constant, we want to multiply it into the coefficient
            if (e.IsNumber)
            {
                coefficient *= e.ToNumber();
            } else
            {
                //otherwise just add it to the list
                list.Add(e);
            }
        }

        /// <summary>
        /// Shorthand to call AddToList for the numerator.
        /// </summary>
        /// <param name="e"></param>
        private void AddToNumerator(TermElement e) => AddToList(e, numerators, ref coefficientNumerator);

        /// <summary>
        /// Shorthand to call AddToList for the denominator.
        /// </summary>
        /// <param name="e"></param>
        private void AddToDenominator(TermElement e) => AddToList(e, denominators, ref coefficientDenominator);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Determines if the given Term is a like Term, when compared to this Term.
        /// 
        /// Terms are like when all the non-constant values match within the two Terms.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if the terms are like, otherwise false.</returns>
        public bool IsLikeTerm(Term other)
        {
            //terms are like if they have the same variables with the same exponents

            //cannot be the same if the non-constant terms are different amounts
            if (numerators.Count != other.numerators.Count || denominators.Count != other.denominators.Count) return false;

            //then actually check the terms

            //terms are not necessarily in order
            foreach(TermElement te in numerators)
            {
                if (!other.numerators.Any(t => t.Equals(te))) return false;
            }

            foreach(TermElement te in denominators)
            {
                if (!other.denominators.Any(t => t.Equals(te))) return false;
            }

            //if it found it all, then it must be a like term
            return true;
        }

        /// <summary>
        /// Finds the highest power within this Term.
        /// </summary>
        /// <returns>The highest power within this Term. 
        /// It can be negative if the variables with powers are on the bottom of the fraction.</returns>
        public Number HighestPower()
        {
            Number n = FindHighestPower(numerators);
            Number d = FindHighestPower(denominators);

            //anything in the denom should be negated, since it is on the bottom part of the fraction
            //so, as long as the numerator returns a value, it should be used
            if (n == double.NaN)
                return d;
            else
                return n;
        }

        /// <summary>
        /// Finds the highest power within the given list of TermElements.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Number FindHighestPower(List<TermElement> list)
        {
            Number highest = double.NaN;

            foreach(TermElement te in list)
            {
                //we don't care about constants, only variables
                if(!te.Element.IsConstant && te.Exponent.IsNumber)
                {
                    Number n = te.Exponent.ToNumber();
                    if(highest == double.NaN || n > highest)
                    {
                        highest = n;
                    }
                }
            }

            return highest;
        }

        /// <summary>
        /// Tests if this Term and the other Term share a common denominator.
        /// The Terms have a common denominator when both Terms denominators match.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>True if the Terms have the same denominator, otherwise false.</returns>
        public bool HasCommonDenominator(Term other)
        {
            //assume arguments are already simplified

            //check for the coeffienct and the count first
            if (coefficientDenominator != other.coefficientDenominator || denominators.Count != other.denominators.Count) return false;

            //now check each element
            foreach(TermElement te in denominators)
            {
                if (!other.denominators.Contains(te)) return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a clone, changes it so the denominator matches the given term, and returns the clone.
        /// The real value of the clone does not differ from this Term.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Term EnsureCommonDenominator(Term other)
        {
            if (HasCommonDenominator(other)) return (Term)Clone();

            //no common denominator
            Term clone = (Term)Clone();

            clone.coefficientNumerator *= other.coefficientDenominator;
            clone.coefficientDenominator *= other.coefficientDenominator;

            foreach(TermElement te in other.denominators)
            {
                clone.AddToNumerator(te);
                clone.AddToNumerator(te);
            }

            //now it has a common denominator
            return (Term)clone.Simplify();
        }

        #endregion

        #region Solving

        public override Token Evaluate(Scope scope)
        {
            //evaluate all items in the numerator and denominator
            //we already know the coefficients are evaluated
            Term clone = (Term)Clone();

            List<Token> num = new List<Token>(numerators.Count);
            List<Token> den = new List<Token>(denominators.Count);

            //evaluate the numerator and denominator
            numerators.ForEach(e => num.Add(e.Evaluate(scope)));
            denominators.ForEach(e => den.Add(e.Evaluate(scope)));

            //multiply all numerators and denominators together
            Token numOut = coefficientNumerator.Clone();
            Token denOut = coefficientDenominator.Clone();

            num.ForEach(t => numOut = numOut.Multiply(t));
            den.ForEach(t => denOut = denOut.Multiply(t));

            //if the num and den outputs are able to evaluate to a whole number, evaluate that
            //otherwise, keep it as a fraction

            //if either num or den are not constant, it cannot be simplified any further
            if (!numOut.IsConstant || !denOut.IsConstant) return CreateFraction((Element)numOut, (Element)denOut);

            //if constant, try to reduce if possible
            Term fraction = (Term)CreateFraction((Element)numOut, (Element)denOut).Simplify();

            //if the denominator is one, just return the numerator
            if (fraction.IsDenominatorOne)
            {
                numOut = fraction.coefficientNumerator;
                fraction.numerators.ForEach(e => numOut = numOut.Multiply(e));
                return numOut;
            } else
            {
                return fraction;
            }
        }

        public override Token Simplify()
        {
            //simplify the coefficients if able
            //treat them like a fraction
            //ensure there are whole numbers

            Term clone = (Term)Clone();

            //TODO: find any constants, add to numerator/denominator

            //first, make sure they are both whole numbers
            int places = Math.Max(Functions.DecimalPlaces(clone.coefficientNumerator).Integer, Functions.DecimalPlaces(clone.coefficientDenominator).Integer);

            if(places > 0)
            {
                clone.coefficientNumerator *= Math.Pow(10, places);
                clone.coefficientDenominator *= Math.Pow(10, places);
            }

            //then divide both by their GCF
            int gcf = Functions.GCF(clone.coefficientNumerator, clone.coefficientDenominator).Integer;

            clone.coefficientNumerator /= gcf;
            clone.coefficientDenominator /= gcf;

            //now that that is done, check the numerator values
            //TODO: combine any variables and add their exponents together
            Dictionary<Element, TermElement> newElements = new Dictionary<Element, TermElement>();

            foreach(TermElement e in numerators)
            {
                //start by simplifying the TermElement
                TermElement simplified = (TermElement)e.Simplify();

                //if exponent is zero, ignore it
                if (simplified.Exponent.IsZero) continue;

                //if it simplifies to a number, multiply it to the coefficient
                if(simplified.Exponent.IsOne && simplified.Element is Number n && !n.HasSymbol)
                {
                    clone.coefficientNumerator *= n;
                    continue;
                }

                TermElement ne;

                if(newElements.TryGetValue(simplified.Element, out ne))
                {
                    //add to the exponent expression
                    //set it again in the dictionary
                    ne = new TermElement(ne.Element, (Element)ne.Exponent.Add(simplified.Exponent).Simplify());

                    //ne = (TermElement)ne.Simplify();

                    newElements[simplified.Element] = ne;
                } else
                {
                    //add whole thing
                    newElements.Add(simplified.Element, simplified);
                }
            }

            //replace the numerators in the clone
            clone.numerators = newElements.Select(e => e.Value).ToList();

            clone.EnsureNomAndDenomAreNotEmpty();

            return clone;
        }

        #endregion

        #region Operations

        public override Token Add(Token token)
        {
            //if it as an expression, add that way
            if(token is Expression e)
            {
                return e.Add((Term)Clone());
            } else if (token is Term t)
            {
                //if it is a term, ensure they share a common denominator, then add and simplify
                Term clone = (Term)Clone();

                //make sure they have a common denomitor if they do not
                Term cdT = t.EnsureCommonDenominator(clone);
                clone = clone.EnsureCommonDenominator(t);

                Element newNumerator;

                
                if(clone.IsLikeTerm(cdT))
                {
                    //if they are like terms, just add the coefficients
                    clone.coefficientNumerator += cdT.coefficientNumerator;
                    return clone;
                }
                else
                {
                    //if not, put them into an expression
                    //then just add the numerators together
                    newNumerator = new Expression(new Term(clone.coefficientNumerator, clone.numerators.ToArray()), new Term(cdT.coefficientNumerator, cdT.numerators.ToArray()));
                    //then put that into the numerator of a new term with the correct values
                    return new Term(Number.One, clone.coefficientDenominator, new TermElement[] { new TermElement(newNumerator, Number.One) }, clone.denominators.ToArray());
                }
            } else
            {
                //if it is anything else, multiply it by the denominator
                //turn the nominator into an expression and add it
                //Term term = new Term(new Element[] { })
                throw new NotImplementedException();
            }
        }

        public override Token Multiply(Token token)
        {
            //if it is an expression, multiply it that way
            throw new NotImplementedException();
        }

        public override Number ToNumber()
        {
            return (Number)coefficientNumerator.Clone();
        }

        #endregion

        public override Token Clone()
        {
            return new Term((Number)coefficientNumerator.Clone(), (Number)coefficientDenominator.Clone(), numerators.Select(e => (TermElement)e.Clone()).ToArray(), denominators.Select(e => (TermElement)e.Clone()).ToArray());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (coefficientNumerator == 0 || coefficientDenominator == 0) return "0";

            if (coefficientNumerator != 1 || coefficientDenominator != 1)
            {
                //if either the numberator or denominator are not 1, write the coefficient
                sb.Append(coefficientNumerator.ToString());

                //only write the denominator if it was not 1
                if (coefficientDenominator != 1)
                {
                    sb.Append(Tokens.Divide_Operator.ToChar());
                    sb.Append(coefficientDenominator.ToString());
                }
            }

            //force write the numerator value if no coefficient was written, or none of the numerator values are not one
            if (sb.Length == 0 || numerators.Any(a => !a.IsOne))
            {
                bool forceWrite = numerators.Count == 1;

                //write the numerator
                foreach (TermElement e in numerators)
                {
                    //only print if the value != 1
                    if (forceWrite || !e.IsOne)
                    {
                        sb.Append(e.ToString());
                    }
                }
            }

            //if the denominator is not 1, write it
            //if(!(denominators is Number n) || (n != 1 && n != 0))
            //{
            //    sb.Append(Tokens.Divide_Operator.ToChar());
            //    sb.Append(denominators.ToString());
            //}

            //if nothing was printed, it must be 1
            if (sb.Length == 0) sb.Append("1");

            return sb.ToString();
        }

        /// <summary>
        /// Creates a new Term that represents a fraction.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static Term CreateFraction(Element numerator, Element denominator)
        {
            return new Term(new Element[] { numerator }, new Element[] { denominator });
        }
    }
}
