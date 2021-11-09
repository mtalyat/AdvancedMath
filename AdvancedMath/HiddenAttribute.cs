using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    public static partial class Functions
    {
        /// <summary>
        /// Used to hide methods in the Functions class so they cannot be used in the equations, and only from code.
        /// </summary>
        class HiddenAttribute : Attribute
        {

        }
    }
}
