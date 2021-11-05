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
        /// Shorthand to create a new Expression that is equal to 1.
        /// </summary>
        public static Expression One => new Expression(Term.One);

        #endregion

        //the Expression is only constant if all Terms within are constant
        public override bool IsConstant => terms.All(t => t.IsConstant);

        public override bool IsNumber => terms.Count == 1 && terms[0].IsNumber;

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
        public Expression(params Token[] tokens) : this(tokens.Select(e => (e is Term t) ? t : new Term((Element)e)).ToArray()) { }

        /// <summary>
        /// Creates a new Expression with the given Terms.
        /// </summary>
        /// <param name="terms"></param>
        public Expression(params Term[] terms)
        {
            this.terms = new List<Term>(terms);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Adds a Term to this Expression.
        /// </summary>
        /// <param name="term"></param>
        private void AddTerm(Term term)
        {
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
            //evaluate each term, then add them together
            Expression clone = (Expression)Clone();

            Token output = Number.Zero;

            foreach(Term t in clone.terms)
            {
                output = output.Add(t.Evaluate(scope));
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

            return clone.Simplify();
        }

        public override Token Multiply(Token token)
        {
            //if the token is another Expression, FOIL all elements
            if (token is Expression e)
            {
                return FOIL(e);
            }

            Expression output = new Expression();

            //otherwise, multiply all terms by the given token
            for (int i = 0; i < terms.Count; i++)
            {
                output.Add(terms[i].Multiply(token));
            }

            //lastly, simplify so that we know it is not all 0's
            return output.Simplify();
        }

        public override Number ToNumber()
        {
            return terms[0].ToNumber();
        }

        #endregion

        #region ToString

        private string TermListToString(List<Term> terms)
        {
            if (!terms.Any()) return string.Empty;

            StringBuilder sb = new StringBuilder();

            sb.Append(string.Join($" {Tokens.Add_Operator.ToChar()} ", terms.Select(t => t.ToString())));

            return sb.ToString();
        }

        public override string ToString()
        {
            //print all items in the numerator over the denominator
            //print parenthesis as needed

            if (!terms.Any()) return string.Empty;

            StringBuilder sb = new StringBuilder();

            sb.Append(TermListToString(terms));

            return sb.ToString();
        }

        public override string ToString(bool wrapInParenthesis)
        {
            if (!wrapInParenthesis || terms.Count == 1) return ToString();

            StringBuilder sb = new StringBuilder();

            sb.Append(Tokens.Open_Parenthesis.ToChar());
            sb.Append(ToString());
            sb.Append(Tokens.Close_Parenthesis.ToChar());

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
