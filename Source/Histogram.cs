﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Upatcher;
using static Helpers;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace UniversalPatcher
{
    public class Histogram
    {
        public Histogram(double[] ColumnHeader, double[] RowHeader)
        {
            LogDatas = new List<CsvData>();
            if (ColumnHeader == null)
            {
                ColumnHeader = new double[1];
                ColumnHeader[0] = 0;
            }
            if (RowHeader == null)
            {
                RowHeader = new double[1];
                RowHeader[0] = 0;
            }
            this.columnHeader = ColumnHeader;
            this.rowHeader = RowHeader;
        }
        public class CsvData
        {
            public CsvData(int columns)
            {
                Values = new double[columns];
            }
            public string RowHeader { get; set; }
            public double[] Values { get; set; }
        }

        public class HitData
        {
            public HitData(int Column, int Row, ushort Decimals)
            {
                this.Column = Column;
                this.Row = Row;
                this.Decimals = Decimals;
                Values = new List<double>();
            }
            public int Column { get; set; }
            public int Row { get; set; }
            public List<double> Values { get; internal set; }
            private ushort Decimals;

            public double CurrentAverage { get; internal set; }
            public void AddValue(double val)
            {
                Values.Add(val);
                CurrentAverage += (val - this.CurrentAverage) / Values.Count;
                //LastUpdated = DateTime.Now;
            }
            //public DateTime LastUpdated { get; internal set; }
/*
            public double Average
            {
                get
                {
                    if (Values.Count == 0)
                        return 0;
                    return Math.Round(Values.Average(), Decimals);
                }
            }
*/
        }

        public class HistogramSetup
        {
            public string CsvSeparator { get; set; }
            public string LogFile { get; set; }
            public string XParameter { get; set; }
            public string YParameter { get; set; }
            public string ValueParameter { get; set; }
            public string SkipParameter { get; set; }
            public double SkipValue { get; set; }
            public int HighColor { get; set; }
            public int MidColor { get; set; }
            public int LowColor { get; set; }
            public double HighValue { get; set; }
            public double LowValue { get; set; }
            public string ColumnHeaders { get; set; }
            public string RowHeaders { get; set; }
            public ushort Decimals { get; set; }
        }

        public class HistSettings
        {
            public HistSettings(string sName, HistogramSetup hset) 
            {
                try
                {
                    Name = sName;
                    Rows = hset.RowHeaders.Split(',').Length;
                    Columns = hset.ColumnHeaders.Split(',').Length;
                    XParameter = hset.XParameter;
                    YParameter = hset.YParameter;
                    ValueParameter = hset.ValueParameter;
                }
                catch (Exception ex)
                {
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(st.FrameCount - 1);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    LoggerBold("Error, Histogram, line " + line + ": " + ex.Message);
                }

            }
            public HistSettings() { }
            public string Name { get; set; }
            public int Rows { get; set; }
            public int Columns { get; set; }
            public string XParameter { get; set; }
            public string YParameter { get; set; }
            public string ValueParameter { get; set; }

        }

        public List<CsvData> LogDatas { get; set; }
        public List<HitData> HitDatas { get; set; }
        public int CsvColumns { get; set; }
        private string CsvSeparator { get; set; }
        public string[] Parameters { get; set; }
        private double[] columnHeader { get; set; } //eg RPM
        private double[] rowHeader { get; set; } //   eg MAP
        public ushort Decimals;

        //For CountHitsIncrement()
        private int col;
        private int row;
        private int skip;
        private int val;
        public int LogDataCount { get { return LogDatas.Count; } }
        public string[] ParseCsvHeader(string headerRow, string separator)
        {
            CsvSeparator = separator;
            Parameters = System.Text.RegularExpressions.Regex.Split(headerRow, System.Text.RegularExpressions.Regex.Escape(separator));
            CsvColumns = Parameters.Length;
            return Parameters;
        }

        public void ParseCsvFile(string fName)
        {
            try
            {
                LogDatas = new List<CsvData>();
                StreamReader sr = new StreamReader(fName);
                string line;
                line = sr.ReadLine(); //Skip header
                while ((line = sr.ReadLine()) != null)
                {
                    CsvData hd = new CsvData(CsvColumns);
                    //string[] rParts = line.Split(CsvSeparator);
                    string[] rParts = System.Text.RegularExpressions.Regex.Split(line, System.Text.RegularExpressions.Regex.Escape(CsvSeparator));
                    for (int p = 0; p < rParts.Length && p < CsvColumns; p++)
                    {
                        double val;
                        if (double.TryParse(rParts[p].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                            hd.Values[p] = val;
                        else
                            hd.Values[p] = 0;
                        //hd.Values[p] = Convert.ToDouble(rParts[p].Trim(), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    LogDatas.Add(hd);
                }
                sr.Close();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Histogram line " + line + ": " + ex.Message);
            }

        }

        public void CountHits(string ColParam, string RowParam, string ValueParam, string SkipParam, double SkipValue, ushort Decimals)
        {
            try
            {
                int col = Array.IndexOf(Parameters, ColParam);
                if (col<0)
                {
                    LoggerBold("Unknown X parameter: " + ColParam);
                    return;
                }
                int row = Array.IndexOf(Parameters, RowParam);
                if (row < 0)
                {
                    LoggerBold("Unknown Y parameter: " + ColParam);
                    return;
                }
                int val = Array.IndexOf(Parameters, ValueParam);
                if (val < 0)
                {
                    LoggerBold("Unknown Value parameter: " + ColParam);
                    return;
                }
                int skip = Array.IndexOf(Parameters, SkipParam);

                HitDatas = new List<HitData>();
                for (int c=0; c < columnHeader.Length;c++)
                {
                    for (int r=0;r<rowHeader.Length;r++)
                    {
                        HitDatas.Add(new HitData(c, r, Decimals));
                    }
                }
                for (int line=0; line < LogDatas.Count; line++)
                {
                    if (skip < 0 || LogDatas[line].Values[skip] >= SkipValue)
                    {
                        double colData = LogDatas[line].Values[col];
                        double rowData = LogDatas[line].Values[row];
                        var nearestColumnValue = columnHeader.OrderBy(x => Math.Abs(colData - x)).First();
                        var nearestRowValue = rowHeader.OrderBy(x => Math.Abs(rowData - x)).First();
                        int c = Array.IndexOf(columnHeader, nearestColumnValue);
                        int r = Array.IndexOf(rowHeader, nearestRowValue);
                        HitData hd = HitDatas.Where(x => x.Column == c).Where(y => y.Row == r).FirstOrDefault();
                        hd.AddValue(LogDatas[line].Values[val]);
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Histogram line " + line + ": " + ex.Message);
            }
        }
        public int CountHitsIncrement(double[] newData, string ColParam, string RowParam, string ValueParam, string SkipParam, double SkipValue, ushort Decimals)
        {
            int retVal = -1;
            try
            {
                if (HitDatas == null)
                {
                    col = Array.IndexOf(Parameters, ColParam);
                    if (col < 0)
                    {
                        LoggerBold("Unknown X parameter: " + ColParam);
                        return retVal;
                    }
                    row = Array.IndexOf(Parameters, RowParam);
                    if (row < 0)
                    {
                        LoggerBold("Unknown Y parameter: " + ColParam);
                        return retVal;
                    }
                    val = Array.IndexOf(Parameters, ValueParam);
                    if (val < 0)
                    {
                        LoggerBold("Unknown Value parameter: " + ColParam);
                        return retVal;
                    }
                    skip = Array.IndexOf(Parameters, SkipParam);
                    HitDatas = new List<HitData>();
                    for (int c = 0; c < columnHeader.Length; c++)
                    {
                        for (int r = 0; r < rowHeader.Length; r++)
                        {
                            HitDatas.Add(new HitData(c, r, Decimals));
                        }
                    }
                }
                if (skip < 0 || newData[skip] >= SkipValue)
                {
                    if (newData[col] == double.MinValue || newData[col] == double.MaxValue)
                        newData[col] = 0;
                    if (newData[row] == double.MinValue || newData[row] == double.MaxValue)
                        newData[row] = 0;
                    double colData = newData[col];
                    double rowData = newData[row];
                    var nearestColumnValue = columnHeader.OrderBy(x => Math.Abs(colData - x)).First();
                    var nearestRowValue = rowHeader.OrderBy(x => Math.Abs(rowData - x)).First();
                    int c = Array.IndexOf(columnHeader, nearestColumnValue);
                    int r = Array.IndexOf(rowHeader, nearestRowValue);
                    for (int h=0; h< HitDatas.Count;h++)
                    {
                        HitData hd = HitDatas[h];
                        if (hd.Column == c && hd.Row == r)
                        {
                            if (newData[val] == double.MinValue || newData[val] == double.MaxValue)
                                hd.AddValue(0);
                            else
                                hd.AddValue(newData[val]);
                            retVal = h;
                            break;
                        }
                    }
                    //HitData hd = HitDatas.Where(x => x.Column == c).Where(y => y.Row == r).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Histogram line " + line + ": " + ex.Message);
            }
            return retVal;
        }

    }
}
