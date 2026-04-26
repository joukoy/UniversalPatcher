using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Helpers;
namespace UniversalPatcher
{
    public class LogConverter
    {
        public LogConverter()
        {
            RowsTo = -1;
            FillEmpty = 0xFF;
        }
        public enum LenSrc
        {
            Datarow,
            Requestrow,
            Datacount,
            Fixed
        }
        public LenSrc AddressSource { get; set; }
        public LenSrc BlockSizeSource { get; set; }
        public int AddressOffset { get; set; }
        public int LenOffset { get; set; }
        public int AddressBytes { get; set; }
        public int LenBytes { get; set; }
        public int FixedLength { get; set; }
        public int DataOffset { get; set; }
        public List<int> RequestBytes { get; set; }
        public List<int> DataBytes { get; set; }
        public int RowsFrom { get; set; }
        public int RowsTo { get; set; }
        public byte FillEmpty { get; set; }
        public int GlobalOffset { get; set; }
        public string GetRequestDetectionsString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i=0;i<RequestBytes.Count;i++)
            {
                if (RequestBytes[i] < 0)
                {
                    sb.Append("* ");
                }
                else
                {
                    sb.Append(RequestBytes[i].ToString("X2") + " ");
                }
            }
            return sb.ToString().Trim();
        }
        public string GetDataDetectionsString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < DataBytes.Count; i++)
            {
                if (DataBytes[i] < 0)
                {
                    sb.Append("* ");
                }
                else
                {
                    sb.Append(DataBytes[i].ToString("X2") + " ");
                }
            }
            return sb.ToString().Trim();
        }
        public void SetDataDetection(string detectStr)
        {
            DataBytes = new List<int>();
            string[] parts = detectStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i=0;i<parts.Length;i++)
            {
                if (HexToInt(parts[i], out int x))
                {
                    DataBytes.Add(x);
                }
                else
                {
                    DataBytes.Add(-1);
                }
            }
        }
        public void SetRequestDetection(string detectStr)
        {
            RequestBytes = new List<int>();
            string[] parts = detectStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                if (HexToInt(parts[i], out int x))
                {
                    RequestBytes.Add(x);
                }
                else
                {
                    RequestBytes.Add(-1);
                }
            }
        }

    }
}
