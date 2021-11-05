using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    /// <summary>
    /// Scopes are a collection of values that can be plugged into variables.
    /// </summary>
    public class Scope
    {
        /// <summary>
        /// Shorthand to create a new Scope with no stored values.
        /// </summary>
        public static Scope Empty => new Scope();

        /// <summary>
        /// A dictionary to store the variables and their corresponding values.
        /// </summary>
        private Dictionary<Variable, Number> variables;

        /// <summary>
        /// Creates a new Scope with no values.
        /// </summary>
        public Scope()
        {
            variables = new Dictionary<Variable, Number>();
        }

        /// <summary>
        /// Gets the corresponding value for the given Variable. If no value exists, an InterpreterException is thrown.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public Number Get(Variable variable)
        {
            Number value;

            if (variables.TryGetValue(variable, out value))
            {
                return value;
            }

            throw new InterpreterException($"Variable \"{variable}\" does not exist in the current scope.");
        }

        /// <summary>
        /// Sets the value for the given Variable.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public void Set(Variable variable, Number value)
        {
            variables[variable] = value;
        }

        /// <summary>
        /// Removes the Variable and its corresponding value from the Scope.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns>True if the Variable existed and was removed, otherwise false.</returns>
        public bool Remove(Variable variable)
        {
            return variables.Remove(variable);
        }
    }
}
