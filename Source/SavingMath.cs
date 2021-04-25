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
        public double getSavingValue(string mathStr, TableData mathTd, double val)
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
            //double  maxVal = getMaxValue(mathTd.DataType);

            //Secant method:
            double gues1 = 10;
            double gues2 = 100;
            if (minVal < 0 && val < 0)
                gues1 = -100;

            double p2, p1, p0;
            int round;
            int stepsCutoff = 100;
            p0= Func(newMath, gues1);
            p1 = Func(newMath, gues2);         
            p2 = p1 - Func(newMath, p1) * (p1 - p0) / (Func(newMath, p1) - Func(newMath, p0));
            if (double.IsInfinity(p2)) p2 = double.MaxValue;
            for (round = 0; System.Math.Abs(p2 - p1) > 0.9999 && round < stepsCutoff; round++)
            {
                p0 = p1;
                p1 = p2;
                p2 = p1 - Func(newMath, p1) * (p1 - p0) / (Func(newMath, p1) - Func(newMath, p0));
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

        private double Func(string mathStr, double val)
        {
            double outVal = double.MaxValue;    //Target is zero, return maxval on error
            try
            {
                string calcStr = mathStr.Replace("x", val.ToString());
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
