using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    /// <summary>
    /// Equations represent two Tokens, on either side of the EQUALS sign.
    /// </summary>
    public class Equation : IMathematical<Equation>
    {
        /// <summary>
        /// The left side of the Equation.
        /// </summary>
        private Token left;

        /// <summary>
        /// The right side of the Equation.
        /// </summary>
        private Token right;

        /// <summary>
        /// Creates a new Equation with the given left and right tokens.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public Equation(Token left, Token right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Solves this Equation for all unknown Variables.
        /// </summary>
        /// <returns></returns>
        public Scope Solve()
        {
            //check out TODO.txt

            return GetScope();
        }

        /// <summary>
        /// Finds all Variables within this Equation, adds them to a Scope and returns it.
        /// </summary>
        /// <returns></returns>
        private Scope GetScope()
        {
            Scope scope = new Scope();
            
            //check out TODO.txt

            return scope;
        }

        public Equation Evaluate(Scope scope)
        {
            Equation clone = Clone();

            clone.left = clone.left.Evaluate(scope);
            clone.right = clone.right.Evaluate(scope);

            return clone;
        }

        public Equation Simplify()
        {
            Equation clone = Clone();

            clone.left = clone.left.Simplify();
            clone.right = clone.right.Simplify();

            return clone;
        }

        public Equation Reduce()
        {
            Equation clone = Clone();

            clone.left = clone.left.Reduce();
            clone.right = clone.right.Reduce();

            return clone;
        }

        public Equation Expand()
        {
            Equation clone = Clone();

            clone.left = clone.left.Expand();
            clone.right = clone.right.Expand();

            return clone;
        }

        public override string ToString()
        {
            //if sides are constant and not equal, print the not equals sign instead
            //we can leave as = if a side is not constant since there is still a variable
            return $"{left} {(left.IsConstant && right.IsConstant && left.ToNumber() != right.ToNumber() ? Symbols.NOT_EQUALS : Symbols.EQUALS)} {right}";
        }

        public Equation Clone()
        {
            return new Equation(left.Clone(), right.Clone());
        }
    }
}
