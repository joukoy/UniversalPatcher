using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using System.Diagnostics;

namespace UniversalPatcher
{
    public class SavingMath
    {
        string mathString;
        public double GetSavingValue(string mathStr, TableData mathTd, double val)
        {
            double minVal = GetMinValue(mathTd.DataType);
            double  maxVal = GetMaxValue(mathTd.DataType);
            mathString = parser.FormatString(mathStr);

            double retVal;
            double calcVal;
            int iterations;
            double errorEstimate;

            double tolerance = 0.01;//Math.Abs(0.01 * val);

            if (mathString.Contains("/x"))
            {
                //Bisect works better when didive by X
                if (minVal == 0)
                    minVal++;   //Avoid divide by zero error
                retVal = RootFinding.Bisect(Func, minVal, maxVal, tolerance, val, out iterations, out errorEstimate); 
                calcVal = Func(retVal);
                Debug.WriteLine("Bisect: " + retVal + ", calculated: " + calcVal + ", iterations: " + iterations);
            }
            else
            {
                retVal = RootFinding.Brent(Func, minVal, maxVal, tolerance, val, out iterations, out errorEstimate);
                calcVal = Func(retVal);
                Debug.WriteLine("Brent: " + retVal + ", calculated: " + calcVal + ", iterations: " + iterations);
            }
            return retVal;
        }


        private double Func(double val)
        {
            double retVal = double.MaxValue;    
            try
            {
                string calcStr = mathString.Replace("x", val.ToString());
                double x = parser.Parse(calcStr, false);
                retVal = x;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }            
            return retVal;
        }
    }
}
