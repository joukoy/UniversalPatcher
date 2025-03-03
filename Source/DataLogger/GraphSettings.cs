using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace UniversalPatcher
{
    class GraphSettings
    {
        public GraphSettings() 
        {
            ZoomStartTime = 0;
            pointDataGroups = new List<PointDataGroup>();
            ZoomEndTime = long.MaxValue;
            zoomStarts = new List<long>();
            zoomEnds = new List<long>();
        }
        public GraphSettings(Chart chart)
        {
            this.chart = chart;
            pointDataGroups = new List<PointDataGroup>();
            ZoomStartTime = 0;
            ZoomEndTime = long.MaxValue;
            zoomStarts = new List<long>();
            zoomEnds = new List<long>();
        }
        private Chart chart;
        public long ZoomStartTime 
        {
            get { return startTime; } 
            set
            {
                startTime = value;
                startPoint = 0;
                for (int p=0; p<pointDataGroups.Count;p++)
                {
                    if (pointDataGroups[p].pointDatas[0].TimeStamp >= value)
                    {
                        startPoint = p;
                        break;
                    }
                }
            }
        }
        public long ZoomEndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
                endPoint = SampleCount - 1; 
                for (int p = 0; p < pointDataGroups.Count; p++)
                {
                    if (pointDataGroups[p].pointDatas[0].TimeStamp >= value)
                    {
                        endPoint = p;
                        break;
                    }
                }
            }
        }
        public long NextStartTime { get; set; }

        public long FirstSampleTime
        {
            get
            {
                if (pointDataGroups.Count == 0)
                    return DateTime.MinValue.Ticks;
                return pointDataGroups[0].pointDatas[0].TimeStamp;
            }
        }
        public long LastSampleTime
        {
            get
            {
                if (pointDataGroups.Count == 0)
                    return DateTime.MinValue.Ticks;
                return pointDataGroups.Last().pointDatas[0].TimeStamp;
            }
        }
        public int ZoomStartPoint
        {
            get
            {
                return startPoint;
            }
            set
            {
                if (SampleCount == 0)
                    ZoomStartTime = 0;
                else
                    ZoomStartTime = pointDataGroups[value].pointDatas[0].TimeStamp;
            }
        }
        public int ZoomEndPoint
        {
            get
            {
                return endPoint;
            }
            set
            {
                if (SampleCount == 0)
                    ZoomEndTime = long.MaxValue;
                else
                    ZoomEndTime = pointDataGroups[value].pointDatas[0].TimeStamp;
            }
        }
        public int ZoomAreaLength { get { return ZoomEndPoint - ZoomStartPoint; } }
        public int SampleCount { get { return pointDataGroups.Count; } }
        public List<long> zoomStarts = new List<long>();
        public List<long> zoomEnds = new List<long>();
        public List<PointDataGroup> pointDataGroups;
        private int startPoint;
        private int endPoint;
        private long startTime;
        private long endTime;
        public void SetNextStartTime(long AddMilliseconds)
        {
            NextStartTime = new DateTime(ZoomStartTime).AddMilliseconds(AddMilliseconds).Ticks;
        }
        public void Forward(long ms)
        {
            int rangeLen = ZoomAreaLength;
            DateTime dtCurrent = new DateTime(ZoomStartTime);
            long target = dtCurrent.AddMilliseconds(ms).Ticks;
            for (int p = ZoomStartPoint; p < pointDataGroups.Count; p++)
            {
                if (pointDataGroups[p].pointDatas[0].TimeStamp >= target)
                {
                    if (p + rangeLen >= SampleCount)
                    {
                        Debug.WriteLine("Zoom end out of range");
                        ZoomStartPoint = SampleCount - rangeLen;
                        ZoomEndPoint = SampleCount - 1;
                    }
                    else
                    {
                        ZoomStartPoint = p;
                        ZoomEndPoint = p + rangeLen;
                    }
                    break;
                }
            }
        }
    }
    class PointData
    {
        public PointData(string pidId, string Pid, long TStamp, double Val, double ScaledVal, int row)
        {
            PidId = pidId;
            PidName = Pid;
            TimeStamp = TStamp;
            Value = Val;
            ScaledValue = ScaledVal;
            Row = row;
        }
        public string PidId { get; set; }
        public string PidName { get; set; }
        public long TimeStamp { get; set; }
        public double Value { get; set; }
        public double ScaledValue { get; set; }
        public int Row { get; set; }
    }

    class PointDataGroup
    {
        public PointDataGroup(int count)
        {
            pointDatas = new PointData[count];
        }
        public PointData[] pointDatas { get; set; }
    }


}
