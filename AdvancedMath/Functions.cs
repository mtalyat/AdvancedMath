using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedMath
{
    public static class Functions
    {
        /// <summary>
        /// Finds the Greatest Common Factor of two numbers.
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static Number GCF(Number one, Number two)
        {
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

        /// <summary>
        /// Returns the amount of decimal placed this number has.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Number DecimalPlaces(Number num)
        {
            string s = num.Value.ToString();

            int index = s.IndexOf('.');

            if(index == -1)
            {
                return 0;//no decimal point, so the amount of places is 0
            }

            return s.Length - 1 - index;
        }
    }
}
