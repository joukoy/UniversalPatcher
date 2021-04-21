using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Symbolics;
using static upatcher;

namespace UniversalPatcher
{
    public class SavingMath
    {
        public Expression SolveSimpleRoot(Expression variable, Expression expr)
        {
            // try to bring expression into polynomial form
            Expression simple = Algebraic.Expand(Rational.Numerator(Rational.Simplify(variable, expr)));

            // extract coefficients, solve known forms of order up to 1
            Expression[] coeff = Polynomial.Coefficients(variable, simple);
            switch (coeff.Length)
            {
                case 1: return Expression.Zero.Equals(coeff[0]) ? variable : Expression.Undefined;
                case 2: return Rational.Simplify(variable, Algebraic.Expand(-coeff[0] / coeff[1]));
                default: return Expression.Undefined;
            }

        }
        public double getSavingValue(string mathStr, double val)
        {
            double retVal = double.MinValue;

            //We need formula which result is zero
            //For example: 
            // 14.7/ ( 0.156 + (((128+x)/2)/128) ) = 12.9
            // ==>>
            // 14.7/ ( 0.156 + (((128+x)/2)/128) ) -12.9

            string newMath = "(" + mathStr.ToLower() + ") - " + val.ToString();
            newMath = newMath.Replace(",", ".");
            var x = Expression.Symbol("x");
            Expression mathExpr = Infix.ParseOrThrow(newMath);
            Expression ax = SolveSimpleRoot(x, mathExpr);
            retVal = Convert.ToDouble(Infix.Format(ax),System.Globalization.CultureInfo.InvariantCulture);

            return retVal;
        }
    }
}
