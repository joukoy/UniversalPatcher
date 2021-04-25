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

            //We need formula which result is zero
            //For example: 
            // 14.7/ ( 0.156 + (((128+x)/2)/128) ) = 12.9
            // ==>>
            // 14.7/ ( 0.156 + (((128+x)/2)/128) ) -12.9

            string newMath =  mathStr.ToLower() + " - " + val.ToString();
            if (val < 0)
                newMath = mathStr.ToLower() + " + " + Math.Abs(val).ToString(); //x*0.33 - -20 => x*0.33 + 20
            double minVal = getMinValue(mathTd.DataType);
            double  maxVal = getMaxValue(mathTd.DataType);
            double gues1 = oldRawVal;
            double gues2;
            if (oldRawVal > ((maxVal - minVal) / 2))
                gues2 = 0.8 * gues1;
            else
                gues2 = 1.1 * gues1;

            mathString = newMath;
            double retVal = secant(gues1, gues2, val, newMath, minVal,maxVal);
            if (double.IsNaN(retVal) || Math.Abs(Func(retVal) - val) > 1)
            {
                mathString = mathStr.ToLower();
                retVal = RootFinding.Brent(Func, minVal, maxVal,0.99, val);
                Debug.WriteLine("Brent: " + retVal);
            }
            if ( double.IsNaN(retVal) || Math.Abs(Func(retVal) - val) > 1)
            {
                mathString = mathStr.ToLower();
                retVal = RootFinding.Bisect(Func, minVal, maxVal, 0.99, val);
                Debug.WriteLine("Bisect: " + retVal);
            }
            return retVal;
        }

        double secant(double gues1, double gues2, double val, string newMath, double min, double max)
        {
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
            double outVal = double.MaxValue;    //Target is zero, return maxval on error
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
