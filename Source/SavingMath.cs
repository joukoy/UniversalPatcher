using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static upatcher;
using System.Diagnostics;

namespace UniversalPatcher
{
    public class SavingMath
    {
        string mathString;
        public double getSavingValue(string mathStr, TableData mathTd, double val, double oldRawVal)
        {

            double minVal = getMinValue(mathTd.DataType);
            double  maxVal = getMaxValue(mathTd.DataType);
            mathString = parser.FormatString(mathStr);
            if (minVal == 0)
                if (mathString.Contains("/x"))
                    minVal++;   //Avoid divide by zero error

            double retVal;
            double calcVal;

            double tolerance = 0.01;//Math.Abs(0.01 * val);

            if (mathString.Contains("/x"))
            {
                //Bisect works better when didive by X
                retVal = RootFinding.Bisect(Func, minVal, maxVal, tolerance, val); 
                calcVal = Func(retVal);
                Debug.WriteLine("Bisect: " + retVal + ", calculated: " + calcVal);
            }
            else
            {
                retVal = RootFinding.Brent(Func, minVal, maxVal, tolerance, val);
                calcVal = Func(retVal);
                Debug.WriteLine("Brent: " + retVal + ", calculated: " + calcVal);
            }
            return retVal;
        }

        double secant(double gues1, double gues2, double val, string newMath, double min, double max)
        {
            //Not used for now
            double p2, p1, p0;
            int round;
            int stepsCutoff = 100;
            p0 = Func( gues1);
            p1 = Func(gues2);
            p2 = p1 - Func(p1) * (p1 - p0) / (Func(p1) - Func(p0));
            if (double.IsInfinity(p2)) p2 = double.MaxValue;
            for (round = 0; System.Math.Abs(p2 - p1) > 0.9999 && round < stepsCutoff; round++)
            {
                p0 = p1;
                p1 = p2;
                p2 = p1 - Func(p1) * (p1 - p0) / (Func(p1) - Func( p0));
                Debug.WriteLine("Secant method round " + round + ", value: " + p2);
            }
            if (round < stepsCutoff)
                return p2;
            else
            {
                System.Diagnostics.Debug.WriteLine("{0}.The Secant method did not converge", p2);
                return double.NaN;
            }
        }

        private double Func(double val)
        {
            double outVal = double.MaxValue;    
            try
            {
                string calcStr = mathString.Replace("x", val.ToString());
                double x = parser.Parse(calcStr, false);
                outVal = x;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
            return outVal;
        }

    }
}
