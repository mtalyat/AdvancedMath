using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AdvancedMath
{
    /// <summary>
    /// The Functions class contains functions that can be called in equations, as well as code.
    /// All Function names are case insensitive.
    /// All Functions must have Token as the parameter type, and some form of Token as the return type.
    /// </summary>
    public static partial class Functions
    {
        private static SortedDictionary<string, MethodInfo> methods;

        static Functions()
        {
            LoadMethods();
        }

        private static void LoadMethods()
        {
            methods = new SortedDictionary<string, MethodInfo>();

            //get all public static methods
            MethodInfo[] allMethods = typeof(Functions).GetMethods(BindingFlags.Public | BindingFlags.Static);

            //only add the ones that do not have the hidden attribute
            foreach(MethodInfo mi in allMethods)
            {
                if(!mi.GetCustomAttributes(typeof(HiddenAttribute), false).Any())
                {
                    methods.Add(mi.Name.ToLower(), mi);
                }
            }
        }

        /// <summary>
        /// Attempts to get a Function based on the given name.
        /// </summary>
        /// <param name="name">The name of the Function, case insensitive.</param>
        /// <param name="function"></param>
        /// <returns>True if the Function was found, false otherwise.</returns>
        [Hidden]
        public static bool TryGetFunction(string name, out Function function)
        {
            MethodInfo mi;

            if(methods.TryGetValue(name.ToLower(), out mi))
            {
                function = new Function(mi);
                return true;
            }

            function = null;
            return false;
        }

        #region General

        /// <summary>
        /// Returns the amount of decimal placed this number has.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Number DecimalPlaces(Token t)
        {
            Number num = t.ToNumber();

            string s = num.Value.ToString();

            int index = s.IndexOf('.');

            if (index == -1)
            {
                return 0;//no decimal point, so the amount of places is 0
            }

            return s.Length - 1 - index;
        }

        #endregion

        #region Algebra

        /// <summary>
        /// Finds the Greatest Common Factor of two numbers.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static Number GCF(Token t1, Token t2)
        {
            Number one = t1.ToNumber();
            Number two = t2.ToNumber();

            int num1 = one.Integer;
            int num2 = two.Integer;
            double gcf = 1;

            for (int i = 2; i <= one && i <= two; i++)
            {
                if(num1 % i == 0 && num2 % i == 0)
                {
                    gcf = i;
                }
            }

            return gcf;
        }

        #endregion

        #region Triginometry

        /// <summary>
        /// Performs the cos function, using the given radians as input.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Number Cos(Token t)
        {
            return Math.Cos(t.ToNumber());
        }

        /// <summary>
        /// Performs the sin function, using the given radians as input.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Number Sin(Token t)
        {
            return Math.Sin(t.ToNumber());
        }

        /// <summary>
        /// Performs the cos function, using the given radians as input.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Number Tan(Token t)
        {
            return Math.Tan(t.ToNumber());
        }

        #endregion
    }
}
