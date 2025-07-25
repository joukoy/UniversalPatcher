using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    public class Hertz
    {
        public Hertz()
        {
            times = new DateTime[300];
        }
        private DateTime[] times;
        private int currentIndex;
        private bool firstRound = true;
        public void AddTime()
        {
            currentIndex++;
            if (currentIndex >= times.Length)
            {
                currentIndex = 0;
                firstRound = false; 
            }
            times[currentIndex] = DateTime.Now;
        }
        public double GetHertz()
        {
/*            if (DateTime.Now.Subtract(times[currentIndex]) > TimeSpan.FromMilliseconds(3000))
            {
                return -1;
            }
*/
            int i = currentIndex;
            int count = 0;
            if (firstRound && currentIndex < 3)
            {
                return -1;
            }
            while (true)
            {
                count++;
                if (i == 0)
                {
                    if (firstRound)
                    {
                        break;
                    }
                    else
                    {
                        i = times.Length;
                    }
                }
                i--;
                if (i == currentIndex + 1)
                {
                    break;
                }
                TimeSpan tSpan = DateTime.Now.Subtract(times[i]);
                if (tSpan > TimeSpan.FromMilliseconds(2000) && count >= 3)
                {
                    //Debug.WriteLine("Count: " + count.ToString() + ", time: " + tSpan.TotalSeconds.ToString());
                    return count / tSpan.TotalSeconds;
                }
            }
            return -1;
        }
    }
}
