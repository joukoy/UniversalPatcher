using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static Upatcher;

namespace UniversalPatcher
{
    public class MathConverter
    {
        public class MList
        {
            public MList()
            {
                mParts = new List<string>();
                opers = new List<char>();
                mParts.Add("");
            }
            public List<string> mParts { get; set; }
            public List<char> opers { get; set; }
        }

        //
        //This converts from PidDesctription scaling to ConversionFactor
        //Note, only special cases covered, not all possible conversions
        //
        public static string ScalingToConversionFactor(PidInfo pi)
        {
            string Unit = pi.Unit.ToUpper();
            string Scaling = pi.Scaling.ToUpper(); ;
            string Descr = pi.Description.ToUpper();
            string math = "";

            if (pi.PidNumber == 5703)
            {
                return GetBitsMath(pi);
            }

            if (pi.PidNumber == 4489)
            {
                math = "Enum: 0:FAULT,1:NO FAULT,2:ABNORMAL SPEED";
                return math;
            }
            if (pi.PidNumber == 4955)
            {
                math = "CircuitStatus";
                return math;
            }
            if (pi.PidNumber == 6493)
            {
                math = "X*(16/65536)";
                return math;
            }
            List<string> bitSearchStrs = new List<string>();
            bitSearchStrs.Add( "BIT MAP AS FOLLOWS ");
            bitSearchStrs.Add( "BITS(");
            for (int x=0; x < 8; x++)
            {
                bitSearchStrs.Add("BIT" + x.ToString());
                bitSearchStrs.Add("BIT " + x.ToString());
            }
            foreach (string bitStr in bitSearchStrs)
            {
                if (Descr.Contains(bitStr))
                {
                    return GetBitsMath(pi);
                }
            }
            if (Scaling == "00=FAIL, 11=PASS, 10 OR 01=INDETERMINATE" || Scaling == "00= FAIL, 11= PASS, 10 OR 01=INDETERMINATE")
            {
                math = "Bits: 1-0=STATUS [00=FAIL, 11=PASS, 10 or 01=INDETERMINATE]";
                return math;
            }

            if (pi.Description.ToLower().StartsWith("byte "))
            {
                math = "Bytes: " + pi.Description;
                return math;
            }
            if (Scaling == "N/A" || Scaling == "None" || Scaling == "no scaling necessary")
            {
                math = "X";
                return math;
            }
            if (Scaling == "16*N/65536")
            {
                math = "16*X*65536";
                return math;
            }
            if (Scaling == "N+255-E/250*100%")
            {
                return "";
            }
            if (Scaling == "Y=RPM/32")
            {
                math = "X*32";
                return math;
            }
            if (Scaling == "N=E*160  (E=DEG OF CAM OFFSET)")
            {
                return "X/160";
            }
            if (Scaling == "SEE TEXT")
            {
                math = "Enum: " + pi.Description.Replace("-", ":").Trim(',');
                return math;
            }
            if (Scaling.Contains("R1") || Scaling == "Look-up Table")
            {
                return "";
            }

            if (Scaling == "N=E* RPM")
            {
                return "X";
            }
            string[] tmpSs = new string[] { "IN(H20SEC","INOFH2O","INH20SEC","%EGROPEN","%TPSOPEN", "GMS/SEC", "GMS/S", "A/F", "DEGREES", "MSEC", "GRAMS/SEC", "GPS", "KPH", "BCDDIGITS", "FTLBS", "FT.LBS", "SEC", "RPM" , "DEG", "??","ADAPT", "ABS", "RANGE IS 0.0 TO 8.0", "RANGE 00-3D HEX", "(RANGE 0-65535)", "(E=DEG OF CAM OFFSET)" };
            List<string>  searchStrs = new List<string>();
            if (!string.IsNullOrEmpty(Unit))
            {
                searchStrs.Add(Unit);
                searchStrs.Add("(" + Unit +")");
            }
            for (int t=0; t<tmpSs.Count(); t++)
            {                
                    searchStrs.Add("(" + tmpSs[t] + ")");
            }
            for (int t = 0; t < tmpSs.Count(); t++)
            {
                    searchStrs.Add(tmpSs[t]);
            }

            if (Scaling.StartsWith("(") && Scaling.EndsWith(")"))
            {
                if (Scaling.Split('(').Length == 1)
                {
                    Scaling = Scaling.Substring(1);
                    Scaling = Scaling.Substring(0, Scaling.Length - 1);
                }
            }
            if (string.IsNullOrEmpty(Scaling))
            {
                return "";
            }
            Scaling = Scaling.Replace(" (N=E)", "");
            string[] parts = Scaling.Replace(" ", "").Split('=');
            if (parts.Length == 1)
            {                
                Debug.WriteLine(Scaling);
                math = "X";
                return math;
            }
            string rightSide = parts[1].ToUpper().Trim();
            if (rightSide == "COUNT" || rightSide == "COUNTS" || rightSide == "STEPS" || rightSide == "SECONDS")
            {
                math =  "X";
                return math;
            }
            if (parts.Length == 3)
            {
                math =  Scaling.Replace(" ", ",");
                return math;
            }
            else if (parts.Length > 3)
            {
                math =  "Enum: " + Scaling.Replace("=", ":").Replace(";",",");
                return math;
            }
            if (parts.Length == 2)
            {
                if (Regex.Replace(parts[1], "[^0-9,.]", "") == parts[1]) //Only numbers & .
                {
                    Debug.WriteLine("Numberonly: " + Scaling);
                    if (parts[1].StartsWith("."))
                        math = "0" + parts[1];
                    else
                        math = parts[1];
                    return math;
                }

                MList lParts = GetMathOpersParts(parts[0], Unit, true);
                string tmp = parts[1].ToUpper().Replace("(DEGC)", "").Trim();
                foreach (string searchStr in searchStrs)
                {
                    string tmp2 = tmp;
                    string[] ops = { "*", "/", "+", "-" };
                    foreach (string op in ops)
                    {
                        if (tmp.EndsWith(op + searchStr))
                        {
                            tmp2 = tmp.Replace(searchStr, "X");
                            continue;
                        }
                    }
                    if (tmp2.EndsWith(searchStr))
                        tmp2 = tmp2.Replace(searchStr, "");
                    if (tmp2.Contains(searchStr))
                        tmp2 = tmp2.Replace(searchStr, "X");
                    if (string.IsNullOrEmpty(tmp2) && !string.IsNullOrEmpty(tmp))
                        tmp = "X";
                    else
                        tmp = tmp2;
                }
                if (parts[1] != tmp)
                {
                    parts[1] = tmp;
                    Debug.WriteLine(parts[1] + " => " + tmp);
                }
                MList rParts = GetMathOpersParts(tmp, Unit, false);
                Application.DoEvents();
                if (rParts.opers.Count == 0 && lParts.opers.Count == 0)
                {
                    math = "X";
                    return math;
                }
                if (lParts.mParts.Contains("N") && rParts.opers.Count == 0)
                {
                    List<string> allParts = GetMathParts(parts[0], Unit);

                    math = "";
                    for (int m = 0; m < allParts.Count; m++)
                    {
                        math += allParts[m];
                    }
                    Debug.WriteLine(Scaling + " => " + math);
                    return math;
                }

                if (rParts.opers.Count == 0)
                {
                    math =  "X";
                    return math;
                }
                if (!lParts.mParts.Contains("N"))
                {
                    List<string> allParts = GetMathParts(parts[1], Unit);

                    math = "";
                    for (int m = 0; m < allParts.Count; m++)
                    {
                        math += allParts[m];
                    }
                    return math;
                }
                else // N=<math>                
                {
                    math = "";

                    switch (rParts.opers.Count)
                    {
                        case 0:
                            math = "X";
                            break;
                        case 1:
                            if (rParts.mParts[1] == "X" || rParts.mParts[1] == "E" || rParts.mParts[1] == "Y")
                            {
                                switch (rParts.opers[0])
                                {
                                    case ('+'):
                                        math = "X-" + rParts.mParts[0];
                                        break;
                                    case ('-'):
                                        math = "X+" + rParts.mParts[0];
                                        break;
                                    case ('*'):
                                        math = "X/" + rParts.mParts[0];
                                        break;
                                    case ('/'):
                                        math = "X/" + rParts.mParts[0];
                                        break;
                                }
                            }
                            else
                            {
                                switch (rParts.opers[0])
                                {
                                    case ('+'):
                                        math = "X-" + rParts.mParts[1];
                                        break;
                                    case ('-'):
                                        math = "X+" + rParts.mParts[1];
                                        break;
                                    case ('*'):
                                        math = "X/" + rParts.mParts[1];
                                        break;
                                    case ('/'):
                                        math = "X*" + rParts.mParts[1];
                                        break;
                                }
                            }
                            break;
                        case 2:
                            if (rParts.mParts[0].ToUpper() == "X")
                            {
                                if (rParts.opers[0] == '*')
                                {
                                    if (rParts.opers[1] == '-')
                                    {
                                        math = "(X+" + rParts.mParts[2] + ")/" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '+')
                                    {
                                        math = "(X-" + rParts.mParts[2] + ")/" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '*')
                                    {
                                        math = "X/" + rParts.mParts[1] + "/" + rParts.mParts[2];
                                    }
                                    else if (rParts.opers[1] == '/')
                                    {
                                        math = "X/" + rParts.mParts[1] + "*" + rParts.mParts[2];
                                    }
                                }
                                else if (rParts.opers[0] == '/')
                                {
                                    if (rParts.opers[1] == '-')
                                    {
                                        math = "(X+" + rParts.mParts[2] + ")*" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '+')
                                    {
                                        math = "(X-" + rParts.mParts[2] + ")*" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '*')
                                    {
                                        math = "X*" + rParts.mParts[1] + "/" + rParts.mParts[2];
                                    }
                                    else if (rParts.opers[1] == '/')
                                    {
                                        math = "X*" + rParts.mParts[1] + "*" + rParts.mParts[2];
                                    }

                                }
                                else if (rParts.opers[0] == '+')
                                {
                                    if (rParts.opers[1] == '-')
                                    {
                                        math = "X-" + rParts.mParts[2] + "+" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '+')
                                    {
                                        math = "X-" + rParts.mParts[2] + "-" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '*')
                                    {
                                        math = "(X-" + rParts.mParts[1] + ")/" + rParts.mParts[2];
                                    }
                                    else if (rParts.opers[1] == '/')
                                    {
                                        math = "(X-" + rParts.mParts[1] + ")*" + rParts.mParts[2];
                                    }

                                }
                                else if (rParts.opers[0] == '-')
                                {
                                    if (rParts.opers[1] == '-')
                                    {
                                        math = "X+" + rParts.mParts[2] + "+" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '+')
                                    {
                                        math = "X+" + rParts.mParts[2] + "+" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '*')
                                    {
                                        math = "X+" + rParts.mParts[1] + "*" + rParts.mParts[2];
                                    }
                                    else if (rParts.opers[1] == '/')
                                    {
                                        math = "X+" + rParts.mParts[1] + "/" + rParts.mParts[2];
                                    }
                                }
                            }
                            else if (rParts.mParts[1].ToUpper() == "X")
                            {
                                if (rParts.opers[0] == '-')
                                {
                                    if (rParts.opers[1] == '-')
                                    {
                                        math = rParts.mParts[0] + "+X+"  + rParts.mParts[2];
                                    }
                                    else if (rParts.opers[1] == '+')
                                    {
                                        math = rParts.mParts[0] + "+X-" + rParts.mParts[2];
                                    }
                                    else if (rParts.opers[1] == '*')
                                    {
                                        math = "(" + rParts.mParts[0] + "+X)/" + rParts.mParts[2];
                                    }
                                    else if (rParts.opers[1] == '/')
                                    {
                                        math = "(" + rParts.mParts[0] + "+X)*" + rParts.mParts[2];
                                    }
                                }
                                else if (rParts.opers[0] == '+')
                                {
                                    if (rParts.opers[1] == '-')
                                    {
                                        math = rParts.mParts[0] + "-X+" + rParts.mParts[2];
                                    }
                                    else if (rParts.opers[1] == '+')
                                    {
                                        math = rParts.mParts[0] + "-X-" + rParts.mParts[2];
                                    }
                                    else if (rParts.opers[1] == '*')
                                    {
                                        math = "(" + rParts.mParts[0] + "-X)/" + rParts.mParts[2];
                                    }
                                    else if (rParts.opers[1] == '/')
                                    {
                                        math = "(" + rParts.mParts[0] + "-X)*" + rParts.mParts[2];
                                    }
                                }
                            }
                            else if (rParts.mParts[2].ToUpper() == "X")
                            {
                                if (rParts.opers[0] == '-')
                                {
                                    if (rParts.opers[1] == '-')
                                    {
                                        math = rParts.mParts[0] + "+" + rParts.mParts[1] + "+X";
                                    }
                                    else if (rParts.opers[1] == '+')
                                    {
                                        math = rParts.mParts[0] + "+" + rParts.mParts[1] + "-X";
                                    }
                                    else if (rParts.opers[1] == '*')
                                    {
                                        math = "(" + rParts.mParts[0] + "+X)/" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '/')
                                    {
                                        math = "(" + rParts.mParts[0] + "+X)*" + rParts.mParts[1];
                                    }
                                }
                                else if (rParts.opers[0] == '+')
                                {
                                    if (rParts.opers[1] == '-')
                                    {
                                        math = rParts.mParts[0] + "-" + rParts.mParts[1] + "+X";
                                    }
                                    else if (rParts.opers[1] == '+')
                                    {
                                        math = rParts.mParts[0] + "-" + rParts.mParts[1] + "-X";
                                    }
                                    else if (rParts.opers[1] == '*')
                                    {
                                        math = "(" + rParts.mParts[0] + "-X)/" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '/')
                                    {
                                        math = "(" + rParts.mParts[0] + "-X)*" + rParts.mParts[1];
                                    }
                                }
                                else if (rParts.opers[0] == '*')
                                {
                                    if (rParts.opers[1] == '-')
                                    {
                                        math = "(X+" + rParts.mParts[0] + ")/" + rParts.mParts[1] ;
                                    }
                                    else if (rParts.opers[1] == '+')
                                    {
                                        math = "(X-" + rParts.mParts[0] + ")/" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '*')
                                    {
                                        math = rParts.mParts[0] + "/" + rParts.mParts[1] +"/" + rParts.mParts[2];
                                    }
                                    else if (rParts.opers[1] == '/')
                                    {
                                        math = rParts.mParts[2] + "/" + rParts.mParts[0] + "*" + rParts.mParts[1];
                                    }
                                }
                                else if (rParts.opers[0] == '/')
                                {
                                    if (rParts.opers[1] == '-')
                                    {
                                        math = "(X+" + rParts.mParts[0] + ")*" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '+')
                                    {
                                        math = "(X-" + rParts.mParts[0] + ")*" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '*')
                                    {
                                        math = rParts.mParts[2] + "/" + rParts.mParts[0] + "/" + rParts.mParts[1];
                                    }
                                    else if (rParts.opers[1] == '/')
                                    {
                                        math = rParts.mParts[0] + "*" + rParts.mParts[1] + "*" + rParts.mParts[2];
                                    }
                                }
                            }
                            else
                            {
                                Debug.WriteLine("X in other position: " + Scaling);
                            }
                            break;
                        case 4:
                            if (rParts.opers[0] == '(' && rParts.opers[2] == ')')
                            {
                                if (rParts.opers[1] == '+')
                                {
                                    if (rParts.opers[3] == '/')
                                    {
                                        math = "X*" + rParts.mParts[2] + "-" + rParts.mParts[1];
                                        //Debug.WriteLine(Scaling + " => " + math);
                                        return math;
                                    }
                                    else if (rParts.opers[3] == '*')
                                    {
                                        math = "X/" + rParts.mParts[2]  + "-" + rParts.mParts[1];
                                        //+"*"+ rParts.mParts[3]
                                        //Debug.WriteLine(Scaling + " => " + math);
                                        return math;
                                    }
                                }
                                else if (rParts.opers[1] == '-')
                                {
                                    if (rParts.opers[3] == '/')
                                    {
                                        math = "X*" + rParts.mParts[2] + "+" + rParts.mParts[1];
                                        //Debug.WriteLine(Scaling + " => " + math);
                                        return math;
                                    }
                                    else if (rParts.opers[3] == '*')
                                    {
                                        math = "X/" + rParts.mParts[2] + "+" + rParts.mParts[1];
                                        //Debug.WriteLine(Scaling + " => " + math);
                                        return math;
                                    }
                                }
                            }
                            if (rParts.opers[0] == '-' && rParts.opers[1] == '('
                                && rParts.opers[2] == '*' && rParts.opers[3] == ')' && rParts.mParts[1] == "X")
                            {
                                math = "(" + rParts.mParts[0] + "+X)/" + rParts.mParts[2];
                                Debug.WriteLine(Scaling + " => " + math);
                                return math;
                            }

                            break;

                        case 5:
                            if (rParts.opers[0] == '(' && rParts.opers[2] == ')' && rParts.opers[3] == '*' && rParts.opers[4] == '/')
                            {
                                if (rParts.opers[1] == '+')
                                    math = "X/" + rParts.mParts[2] + "*" + rParts.mParts[3] + "-" + rParts.mParts[1];
                                else if (rParts.opers[1] == '-')
                                    math = "X/" + rParts.mParts[2] + "*" + rParts.mParts[3] + "+" + rParts.mParts[1];
                                return math;
                            }
                            break;
                        default:
                            math = "";
                            if (!parts[1].Contains("+") && !parts[1].Contains("-") && rParts.mParts[0] == "X")
                            {
                                char o = '/';
                                if (rParts.opers[0] == '/')
                                    o = '*';
                                math = rParts.mParts[0] + o + parts[1].Substring(2);
                            }
                            Debug.WriteLine(Scaling +" => " + math);
                            break;
                    }
                    return math;
                }
            }
            return math;
        }

        private static string GetBitsMath(PidInfo pi)
        {
            StringBuilder sb = new StringBuilder("Bits: ");
            string[] bitVals;
            if (pi.Description.ToUpper().Contains("BITS("))
            {
                string bitDefs = pi.Description.ToUpper().Substring(pi.Description.ToUpper().IndexOf("BITS("));
                bitVals = bitDefs.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                List<string> bitSearchStrs = new List<string>();
                for (int x = 0; x < 31; x++)
                {
                    bitSearchStrs.Add("BIT" + x.ToString());
                    bitSearchStrs.Add("BIT " + x.ToString());
                }
                List<string> tmpVals = new List<string>();
                string descr = pi.Description.ToUpper();
                if (pi.PidNumber == 5703)
                {
                    descr = pi.Scaling.ToUpper();                    
                }
                int i = 0;
                bool firstHit = true;
                for (; i<descr.Length;i++)
                {
                    foreach (string bStr in bitSearchStrs)
                    {
                        if ((i+bStr.Length)>=descr.Length || descr.Substring(i,bStr.Length) == bStr)
                        {
                            if (i>4 && !firstHit)
                            {
                                tmpVals.Add(descr.Substring(3, i - 3).Trim());
                            }
                            firstHit = false;
                            descr = descr.Substring(i).Trim();
                            i = 0;
                            break;
                        }
                    }
                }
                bitVals = tmpVals.ToArray();
            }
            foreach(string bitStr in bitVals)
            {
                char firstChar = bitStr[0];
                if (firstChar >= '0' && firstChar <= '9')
                {
                    string bitStr2 = bitStr.Trim(',').Replace(",", ";");
                    sb.Append("BIT " + bitStr2 + ",");
                }
            }
            string retVal = sb.ToString().Trim().Trim(',');
            if (pi.PidNumber == 5703)
            {
                retVal += " (0=No Fault, 1=Fault)";
            }
            else if(!string.IsNullOrEmpty(pi.Scaling) && pi.Scaling.Split('=').Length > 2)
            {
                retVal += " [" + pi.Scaling + "]";
            }
            Debug.WriteLine(retVal);
            return retVal;
        }

        private static MList GetMathOpersParts(string Math, string Unit, bool Left)
        {
            MList mlist = new MList();
            bool FirstChar = true;
            int i = 0;
            if (!string.IsNullOrEmpty(Unit))
            {
                Math = Math.ToUpper().Replace("(" + Unit + ")", "");
            }
            if (Math.StartsWith("(") && Math.EndsWith(")") && Math.Split('(').Length == 2) 
            {
                Math = Math.Substring(1, Math.Length - 2);
            }

            foreach (char c in Math.Replace("?", ""))
            {
                if (c == '*' || c == '/' || c == '+' || c == '-' || c == '(' || c == ')')
                {
                    if (c == '(' || c == ')')
                    {
                        if (!Math.Contains("+") && !Math.Contains("-"))
                        {
                            //Debug.WriteLine("Brackets not necessary");
                            continue;   //Brackets are not required if dont have any plus/minus calcs
                        }
                    }
                    mlist.opers.Add(c);
                    if (FirstChar && c != '(')
                    {
                        mlist.mParts[0] = "X";
                        mlist.mParts.Add("");
                        i++;
                    }
                    else if (!string.IsNullOrEmpty(mlist.mParts[i]))
                    {
                        mlist.mParts.Add("");
                        i++;
                    }
                }
                else if (c == ' ')
                {
                    mlist.mParts.Add("");
                    i++;
                }
                else 
                {
                    if (i < 0)
                    {
                        i++;
                        mlist.mParts.Add(c.ToString());
                    }
                    else
                    {
                        mlist.mParts[i] += c;
                    }
                }
                FirstChar = false;
            }
            int o = 0;
            while ((o+2) < mlist.opers.Count)
            {
                if (mlist.opers[o] == '(' && (mlist.opers[o+1] == '*' || mlist.opers[o+1] == '/') && mlist.opers[o+2] == ')')
                {
                    mlist.opers.RemoveAt(o + 2);
                    mlist.opers.RemoveAt(o);
                }
                o++;
            }
            string[] searchStrs = new string[] { Unit, "%EGROPEN", "%TPSOPEN", "N", "Y", "E", "%", "GMS/SEC", "GMS/S", "A/F", "DEGREES", "MSEC", "GRAMS/SEC", "GPS", "KPH", "BCDDIGITS", "FTLBS", "FT.LBS", "SEC", "RPM", "VOLTS"};

            for (int m = 0; m < mlist.mParts.Count; m++)
            {
                foreach (string searchStr in searchStrs)
                {
                    if (string.IsNullOrEmpty(searchStr))
                    {
                        continue;
                    }
                    if (Left && searchStr == "N")
                    {
                        continue;
                    }
                    string tmp2 = mlist.mParts[m].ToUpper();
                    if (mlist.mParts[m].ToUpper() == searchStr)
                    {
                        mlist.mParts[m] = "X";
                        break;
                    }
                }

                if (mlist.mParts[m].StartsWith("."))
                    mlist.mParts[m] = "0" + mlist.mParts[m];
            }

            return mlist;
        }

        private static List<string> GetMathParts(string Math, string Unit)
        {
            List<string> allParts = new List<string>();
            bool FirstChar = true;
            int i = -1;
            foreach (char c in Math.Replace("?", ""))
            {
                if (c == '*' || c == '/' || c == '+' || c == '-' || c == '(' || c == ')')
                {
                    if (!FirstChar || c == '(')
                    {
                        allParts.Add(c.ToString());
                        allParts.Add("");
                        i += 2;
                    }
                }
                else if (c != ' ')
                {
                    if (i < 0)
                    {
                        i++;
                        allParts.Add(c.ToString());
                    }
                    else
                    {
                        allParts[i] += c.ToString();
                    }
                }
                FirstChar = false;
            }
            string[] searchStrs = new string[] { Unit, "%EGROPEN", "%TPSOPEN", "N", "Y", "E", "%", "GMS/SEC", "GMS/S", "A/F", "DEGREES", "MSEC", "GRAMS/SEC", "GPS", "KPH", "BCDDIGITS", "FTLBS", "FT.LBS", "SEC", "RPM" };

            for (int m = 0; m < allParts.Count; m++)
            {
                foreach (string searchStr in searchStrs)
                {
                    if (string.IsNullOrEmpty(searchStr))
                    {
                        continue;
                    }
                    string tmp2 = allParts[m].ToUpper();
                    if (allParts[m].ToUpper() == searchStr)
                    {
                        allParts[m] = "X";
                        break;
                    }
                }

                if (allParts[m].StartsWith("."))
                    allParts[m] = "0" + allParts[m];
            }
            return allParts;
        }
    }
}
