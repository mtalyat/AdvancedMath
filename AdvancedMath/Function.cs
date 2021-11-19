using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AdvancedMath
{
    public class Function : Element
    {
        //only constant if all arguments are constant
        public override bool IsConstant => arguments.All(a => a.IsConstant);

        //never a number
        public override bool IsNumber => false;

        public override bool IsOne => false;

        public override bool IsZero => false;

        //functions are not inherently negative
        public override bool IsNegative => false;

        public string Name { get; private set; }

        public int ParameterCount => methodInfo.GetParameters().Length;

        public override bool HasConstantOrVariable => arguments.Any(a => a.HasConstantOrVariable);

        protected MethodInfo methodInfo;

        protected List<Token> arguments;

        #region Constructors

        public Function(MethodInfo method) : this(method, new Token[0]) { }

        public Function(MethodInfo method, params Token[] args)
        {
            methodInfo = method;
            arguments = new List<Token>(args);

            //make the first char lowercase in the name
            Name = char.ToLower(method.Name[0]) + method.Name.Substring(1);
        }

        #endregion

        #region General

        public override Token Expand()
        {
            //cannot be expanded
            return Clone();
        }

        #endregion

        #region Helper Methods

        public void AddArgument(Token arg)
        {
            arguments.Add(arg);
        }

        public void AddArguments(Token[] args)
        {
            arguments.AddRange(args);
        }

        #endregion

        #region Solving

        public override Token Evaluate(Scope scope)
        {
            //if all arguments were properly evaluated, then run the method
            //if the method returns a whole number, return it
            //otherwise return a new Function with the new evaluated arguments

            Function clone = (Function)Clone();

            clone.arguments = arguments.Select(a => a.Evaluate(scope)).ToList();

            if (clone.IsConstant)
            {
                Token output = (Token)methodInfo.Invoke(null, clone.arguments.ToArray());

                if(output.ToNumber().IsWholeNumber)
                {
                    return output;
                }
            }

            return clone;
        }

        public override Token Simplify()
        {
            //functions do not simplify
            //only simplify the arguments
            Function clone = (Function)Clone();

            clone.arguments = arguments.Select(a => a.Simplify()).ToList();

            return clone;
        }

        public override Token Reduce()
        {
            //cannot reduce anymore
            return Clone();
        }

        public override Number ToNumber()
        {
            //if the function evaluates to a number, and the inputs are all constants, it can be done
            if(methodInfo.ReturnType == typeof(Number) && IsConstant)
            {
                return (Number)methodInfo.Invoke(null, arguments.Select(a => a.Evaluate(Scope.Empty)).ToArray());
            } else
            {
                //otherwise, it cannot be made into a number
                return Number.NaN;
            }
        }

        #endregion

        #region Operations

        public override Token Add(Token token)
        {
            if(token is Expression e)
            {
                return e.Add(Clone());
            } else
            {
                return new Expression(new Term((Function)Clone()), new Term(token));
            }
        }

        public override Token Multiply(Token token)
        {

            if (token is Expression e)
            {
                return e.Multiply(Clone());
            }
            else if (token is Term t)
            {
                return t.Multiply(Clone());
            }
            else
            {
                //must multiply into a term if anything else
                return new Term(new Element[] { (Element)Clone(), (Element)token });
            }
        }

        #endregion

        public override Token Clone()
        {
            return new Function(methodInfo, arguments.Select(a => a.Clone()).ToArray());
        }

        public override string ToString()
        {
            return $"{Name}({string.Join(", ", arguments.Select(a => a.Reduce().ToString()).ToArray())})";
        }
    }
}
