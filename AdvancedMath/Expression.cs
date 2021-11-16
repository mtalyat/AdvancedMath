using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    /// <summary>
    /// Expressions combine fractions and expressions. 
    /// Expressions have a numerator and a denominator, each of which are lists of terms.
    /// Each term in the Expression is added together.
    /// 
    /// Handles the addition and subtraction operations.
    /// </summary>
    public class Expression : Element
    {
        #region Common Expressions

        /// <summary>
        /// Shorthand to create a new Expression that is equal to 0.
        /// </summary>
        public static Expression Zero => new Expression(Term.Zero);

        /// <summary>
        /// Shorthand to create a new Expression that is equal to 1.
        /// </summary>
        public static Expression One => new Expression(Term.One);

        #endregion

        //the Expression is only constant if all Terms within are constant
        public override bool IsConstant => terms.All(t => t.IsConstant);

        public override bool HasConstantOrVariable => terms.Any(t => t.HasConstantOrVariable);

        public override bool IsNumber => terms.Count == 1 && terms[0].IsNumber;

        public override bool IsOne => false;

        public override bool IsZero => false;

        //expressions are not inherently negative
        public override bool IsNegative => false;

        private List<Term> terms;

        #region Constructors

        /// <summary>
        /// Creates a new Expression equal to 0.
        /// </summary>
        public Expression()
        {
            terms = new List<Term>(new Term[1] { Term.Zero });
        }

        /// <summary>
        /// Creates a new Expression with the given Tokens.
        /// </summary>
        /// <param name="tokens"></param>
        public Expression(params Token[] tokens) : this(tokens.Select(e => new Term(e)).ToArray()) { }

        /// <summary>
        /// Creates a new Expression with the given Terms.
        /// </summary>
        /// <param name="terms"></param>
        public Expression(params Term[] terms)
        {
            this.terms = new List<Term>(terms);

            //if there are no terms, we need at least something in here
            if(!this.terms.Any())
            {
                AddTerm(Term.Zero);
            }

            SortTerms();
        }

        #endregion

        #region General

        public override Token Expand()
        {
            //expand each term inside of terms
            Expression clone = (Expression)Clone();

            for (int i = 0; i < clone.terms.Count; i++)
            {
                clone.terms[i] = (Term)clone.terms[i].Expand();
            }

            return clone;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Adds a Term to this Expression.
        /// </summary>
        /// <param name="term"></param>
        private void AddTerm(Term term)
        {
            if(terms.Count == 1 && terms[0].IsZero)
            {
                terms.Clear();
            }

            terms.Add(term);
        }

        /// <summary>
        /// Sorts all of the terms based on their exponents.
        /// Higher exponent values go on the left.
        /// </summary>
        private void SortTerms()
        {
            SortedDictionary<double, List<Term>> sorted = new SortedDictionary<double, List<Term>>();

            foreach(Term t in terms)
            {
                //sort the Term based on it's highest power
                Number power = t.HighestPower();

                //if the list does not exist yet, make it
                List<Term> list;
                if(!sorted.TryGetValue(power, out list))
                {
                    list = new List<Term>();
                    sorted[power] = list;
                }
                
                //add it to the corresponding list
                list.Add(t);
            }

            //finally, put the list back together
            terms.Clear();
            foreach(var pair in sorted)
            {
                foreach(Term t in pair.Value)
                {
                    terms.Add(t);
                }
            }
            terms.Reverse();
        }

        /// <summary>
        /// Applies the FOIL method to this Expression and the given Expression.
        /// Returns a new Expression with the result.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Expression FOIL(Expression other)
        {
            Expression output = new Expression();

            //multiply each token by each of the other tokens inside the other expression
            for (int i = 0; i < terms.Count; i++)
            {
                Term t = terms[i];
                for (int j = 0; j < other.terms.Count; j++)
                {
                    Term m = other.terms[j];

                    output.AddTerm((Term)t.Multiply(m));
                }
            }

            //lastly, simplify it
            return (Expression)output.Simplify();
        }

        #endregion

        #region Solving

        public override Token Evaluate(Scope scope)
        {
            //if there are no terms, it is zero
            if (!terms.Any()) return Number.Zero;

            //otherwise, just add all the terms together
            Token output = terms[0].Evaluate(scope);

            for (int i = 1; i < terms.Count; i++)
            {
                output = output.Add(terms[i].Evaluate(scope));
            }

            return output;
        }

        public override Token Simplify()
        {
            //go through each element, similar to a term
            //combine like terms
            List<Term> ts = new List<Term>(terms.Count);

            foreach(Term t in terms)
            {
                Term simplified = (Term)t.Simplify();

                //if a term is 0, forget about it
                if (simplified.IsZero) continue;

                bool found = false;

                for (int i = 0; i < ts.Count; i++)
                {
                    Term tsT = ts[i];

                    if(tsT.IsLikeTerm(simplified))
                    {
                        ts[i] = (Term)tsT.Add(simplified);
                        found = true;
                        break;
                    }
                }

                if(!found)
                {
                    ts.Add(simplified);
                }
            }

            //return a new expression with the simplified terms
            Expression output = new Expression(ts.ToArray());

            output.SortTerms();
            return output;
        }

        public override Token Reduce()
        {
            if (terms.Count > 1) return Clone();

            return terms[0].Reduce();
        }

        #endregion

        #region Operations

        public override Token Add(Token token)
        {
            Expression clone = (Expression)Clone();

            if (token is Term t)
            {
                //if it is already a term, just add it
                clone.AddTerm(t);
            }
            else if (token is Expression expr)
            {
                //if it is an expression, just combine them
                return new Expression(clone.terms.Concat(expr.terms).ToArray());
            }
            else if (token is Element e)
            {
                //if it is not already a term, make it into one and then add it
                clone.AddTerm(new Term(e));
            }
            else
            {
                throw new ArgumentException("The given Token to add must be a Term, or an Element.");
            }

            return clone;
        }

        public override Token Multiply(Token token)
        {
            token = token.Reduce();

            if(token is Term t)
            {
                //if the other one is a term, add this to it's numerator
                return t.Multiply(Clone());
            }

            //otherwise throw this into a term
            //expand will handle multiplying out the tokens with this Expression
            return new Term(new Token[] { Clone(), token });
        }

        public override Number ToNumber()
        {
            Number output = Number.One;

            terms.ForEach(t => output *= t.ToNumber());

            return output;
        }

        #endregion

        #region ToString

        private string TermListToString(List<Term> terms)
        {
            if (!terms.Any()) return string.Empty;

            StringBuilder sb = new StringBuilder();

            //add the initial term
            sb.Append(terms[0].ToString());

            //if there are more, determine whether to print - or +
            for (int i = 1; i < terms.Count; i++)
            {
                Term t = terms[i];

                if(t.IsNegative)
                {
                    sb.Append(" - ");
                    sb.Append(t.ToString(false));
                } else
                {
                    sb.Append(" + ");
                    sb.Append(t.ToString(false));
                }
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            //print all items in the numerator over the denominator
            //print parenthesis as needed

            if (!terms.Any()) return string.Empty;

            StringBuilder sb = new StringBuilder();

            sb.Append(TermListToString(terms));

            //if nothing, the expression is zero
            if (sb.Length == 0) sb.Append('0');

            return sb.ToString();
        }

        public override string ToStringParentheses(bool wrapInParenthesis)
        {
            if (!wrapInParenthesis) return ToString();

            StringBuilder sb = new StringBuilder();

            sb.Append(Tokens.OPEN_PARENTHESIS);
            sb.Append(ToString());
            sb.Append(Tokens.CLOSE_PARENTHESIS);

            return sb.ToString();
        }

        #endregion

        public override Token Clone()
        {
            Expression clone = new Expression();

            clone.terms = terms.Select(t => (Term)t.Clone()).ToList();

            return clone;
        }
    }
}
