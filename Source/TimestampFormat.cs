using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace UniversalPatcher
{
    public class TimestampFormat
    {
        public TimestampFormat(DateTime BaseTime)
        {
            this.BaseTime = BaseTime;
            StampParts = new List<TimeStampPart>();
            string[] tParts = AppSettings.LogImportTimeStampFormat.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries);
            for (int t = 0; t < tParts.Length; t++)
            {
                TimeStampPart tsp;
                tsp = (TimeStampPart)Enum.Parse(typeof(TimeStampPart), tParts[t]);
                StampParts.Add(tsp);
            }
        }
        public enum TimeStampPart
        {
            Year,
            Month,
            Day,
            Hour,
            AmPm,
            Minute,
            Second,
            SecondDecimal,
            Millisecond,
            Unix,
            Ticks,
            Skip
        }

        private List<TimeStampPart> StampParts;
        private DateTime BaseTime;

        public DateTime ConvertTimeStamp(string StampString)
        {
            try
            {
                DateTime retVal = BaseTime;
                if (StampParts.Count == 2 && StampParts[0] == TimeStampPart.Second && (StampParts[1] == TimeStampPart.SecondDecimal || StampParts[1] == TimeStampPart.Millisecond))
                {
                    double val = Convert.ToDouble(StampString, System.Globalization.CultureInfo.InvariantCulture);
                    return BaseTime.AddSeconds(val);
                }
                string[] tParts = StampString.Split(new char[] { ' ', '.', ':', '-', ';' });
                if (tParts.Length != StampParts.Count)
                {
                    //LoggerBold("TimestampFormat: " + AppSettings.LogImportTimeStampFormat + " not match timestamp: " + StampString);
                    return BaseTime;
                }
                if (AppSettings.LogImportTimeStampElapsed)
                {
                    for (int i = 0; i < tParts.Length; i++)
                    {
                        string numbers = Regex.Replace(tParts[i], "[a-zA-Z]", "").Trim();
                        double val = 0;
                        if (!string.IsNullOrEmpty(numbers))
                            val = Convert.ToDouble(numbers);
                        switch (StampParts[i])
                        {
                            case TimeStampPart.Skip:
                                break;
                            case TimeStampPart.Year:
                                retVal = retVal.AddYears((int)val);
                                break;
                            case TimeStampPart.Month:
                                retVal = retVal.AddMonths((int)val);
                                break;
                            case TimeStampPart.Day:
                                retVal = retVal.AddDays((int)val);
                                break;
                            case TimeStampPart.Hour:
                                if (tParts[i].ToLower().Contains("p"))
                                    val += 12;
                                retVal = retVal.AddHours((int)val);
                                break;
                            case TimeStampPart.AmPm:
                                if (tParts[i].ToLower().Contains("p"))
                                    retVal = retVal.AddHours(12);
                                break;
                            case TimeStampPart.Minute:
                                retVal = retVal.AddMinutes((int)val);
                                break;
                            case TimeStampPart.Second:
                                retVal = retVal.AddSeconds((int)val);
                                break;
                            case TimeStampPart.Millisecond:
                                retVal = retVal.AddMilliseconds((int)val);
                                break;
                            case TimeStampPart.SecondDecimal:
                                string tmp = "0." + tParts[i];
                                val = Convert.ToDouble(tmp, System.Globalization.CultureInfo.InvariantCulture) * 1000;
                                retVal = retVal.AddMilliseconds((int)val);
                                break;
                            case TimeStampPart.Unix:
                                retVal = BaseTime.AddMilliseconds(val).ToLocalTime();
                                break;
                            case TimeStampPart.Ticks:
                                long ticks = Convert.ToInt64(tParts[0]);
                                retVal = BaseTime.AddTicks(ticks);
                                break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < tParts.Length; i++)
                    {
                        string numbers = Regex.Replace(tParts[i], "[a-zA-Z]", "").Trim();
                        double val = 0;
                        if (!string.IsNullOrEmpty(numbers))
                            val = Convert.ToDouble(numbers);
                        switch (StampParts[i])
                        {
                            case TimeStampPart.Skip:
                                break;
                            case TimeStampPart.Year:
                                retVal = new DateTime((int)val, retVal.Month, retVal.Day, retVal.Hour, retVal.Minute, retVal.Second, retVal.Millisecond);
                                break;
                            case TimeStampPart.Month:
                                retVal = new DateTime(retVal.Year, (int)val, retVal.Day, retVal.Hour, retVal.Minute, retVal.Second, retVal.Millisecond);
                                break;
                            case TimeStampPart.Day:
                                retVal = new DateTime(retVal.Year, retVal.Month, (int)val, retVal.Hour, retVal.Minute, retVal.Second, retVal.Millisecond);
                                break;
                            case TimeStampPart.Hour:
                                if (tParts[i].ToLower().Contains("p"))
                                    val += 12;
                                retVal = new DateTime(retVal.Year, retVal.Month, retVal.Day, (int)val, retVal.Minute, retVal.Second, retVal.Millisecond);
                                break;
                            case TimeStampPart.AmPm:
                                if (tParts[i].ToLower().Contains("p"))
                                    retVal = retVal.AddHours(12);
                                break;
                            case TimeStampPart.Minute:
                                retVal = new DateTime(retVal.Year, retVal.Month, retVal.Day, retVal.Hour, (int)val, retVal.Second, retVal.Millisecond);
                                break;
                            case TimeStampPart.Second:
                                retVal = new DateTime(retVal.Year, retVal.Month, retVal.Day, retVal.Hour, retVal.Minute, (int)val, retVal.Millisecond);
                                break;
                            case TimeStampPart.Millisecond:
                                retVal = new DateTime(retVal.Year, retVal.Month, retVal.Day, retVal.Hour, retVal.Minute, retVal.Second, (int)val);
                                break;
                            case TimeStampPart.SecondDecimal:
                                string tmp = "0." + tParts[i];
                                val = Convert.ToDouble(tmp, System.Globalization.CultureInfo.InvariantCulture) * 1000;
                                retVal = new DateTime(retVal.Year, retVal.Month, retVal.Day, retVal.Hour, retVal.Minute, retVal.Second, (int)val);
                                break;
                            case TimeStampPart.Unix:
                                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                                retVal = dtDateTime.AddMilliseconds(val).ToLocalTime();
                                break;
                            case TimeStampPart.Ticks:
                                long ticks = Convert.ToInt64(tParts[0]);
                                retVal = new DateTime(ticks);
                                break;
                        }
                    }
                }
                return retVal;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("Error, TimestampFormat line " + line + ": " + ex.Message + ", inner exception: " + ex.InnerException);
            }
            return BaseTime;
        }
    }
}
