using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    public interface IMathematical<T>
    {
        /// <summary>
        /// Evaluates the <typeparamref name="T"/> to the best ability it can, given the scope, and returns it.
        /// Evaluate will not necessarily retain the same object type.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns>An evaluated version of this Token.</returns>
        T Evaluate(Scope scope);

        /// <summary>
        /// Returns a copy of this <typeparamref name="T"/>, simplified.
        /// Simplify will retain the same object type.
        /// </summary>
        /// <returns>A simplified copy of this Token.</returns>
        T Simplify();

        /// <summary>
        /// Reduces this <typeparamref name="T"/> to another type, if able.
        /// Ex. A Term with just a Variable in it will return the Variable.
        /// </summary>
        /// <returns></returns>
        T Reduce();

        /// <summary>
        /// Expands the <typeparamref name="T"/>, if able. 
        /// The Token will retain the same object type.
        /// </summary>
        /// <returns></returns>
        T Expand();

        /// <summary>
        /// Returns a clone of this <typeparamref name="T"/>.
        /// </summary>
        /// <returns></returns>
        T Clone();
    }
}
