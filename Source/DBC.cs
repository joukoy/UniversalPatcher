using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static Helpers;
using static LoggerUtils;

namespace UniversalPatcher
{
    public class DBC
    {
        public List<DBCDescription> Descriptions = new List<DBCDescription>();
        public class DBCMsg
        {
            public DBCMsg(string fName)
            {
                Signals = new List<DBCSignal>();
                FileName = fName;
            }
            public uint CANid;
            public string MessageName { get; set; }
            public int DataLength { get; set; }
            public List<DBCSignal> Signals;
            public string Sender { get; set; }
            public string Description { get; set; }
            public string FileName { get; set; }
        }
        public class DBCSignal
        {
            public DBCSignal()
            {
                MSB = true;
            }
            public string SignalName { get; set; }
            public int BitStartPos { get; set; }
            public int NumBits { get; set; }
            public bool MSB { get; set; }
            public bool Signed { get; set; }
            public string Expression { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
            public string Unit { get; set; }
            public string Receiver { get; set; }
            public string Description { get; set; }
        }

        public class DBCDescription
        {
            public string CANid { get; set; }
            public string Type { get; set; }
            public string Signal { get; set; }
            public string Description { get; set; }
        }

        public static List<DBCMsg> ReadDbcFile(string fName)
        {
            List<string> enumFuncs = new List<string>();
            List<string> enumVals = new List<string>();
            List<DBCMsg> retVal = new List<DBCMsg>();
            try
            {
                StreamReader sr = new StreamReader(fName);
                string line;
                DBCMsg dbcMsg = new DBCMsg(fName);
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("BO_ "))
                    {
                        dbcMsg = new DBCMsg(fName);
                        retVal.Add(dbcMsg);
                        string[] sPart = line.Trim().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (sPart.Length > 1)
                        {
                            string[] uParts = sPart[0].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (uParts.Length < 3)
                            {
                                continue;
                            }
                            if (UInt32.TryParse(uParts[1], out uint id))
                            {
                                dbcMsg.CANid = id;
                            }
                            dbcMsg.MessageName = uParts[2];
                            string[] lParts = sPart[1].Trim().Split(' ');
                            if (lParts.Length > 1)
                            {
                                //dbcMsg = new DBCMsg(fName);
                                if (Int32.TryParse(lParts[0], out int len))
                                {
                                    dbcMsg.DataLength = len;
                                }
                                dbcMsg.Sender = lParts[1];
                            }
                        }
                    }
                    else if (line.Trim().StartsWith("SG_ "))
                    {
                        DBCSignal signal = new DBCSignal();
                        string[] sPart = line.Trim().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (sPart.Length > 1)
                        {
                            signal.SignalName = sPart[0].Substring(4);

                            string[] lParts = sPart[1].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (lParts.Length > 3)
                            {
                                for (int p = 0; p < lParts.Length; p++)
                                {
                                    switch (p)
                                    {
                                        case 0: //Example: 2|11@0-
                                            string[] bParts = lParts[p].Split(new char[] { '|', '@' }, StringSplitOptions.RemoveEmptyEntries);
                                            if (bParts.Length < 3)
                                            {
                                                LoggerBold("Unknown signal definition: " + line);
                                                continue;
                                            }
                                            if (int.TryParse(bParts[0], out int pos))
                                            {
                                                signal.BitStartPos = pos;
                                            }
                                            if (int.TryParse(bParts[1], out int bits))
                                            {
                                                signal.NumBits = bits;
                                            }
                                            if (bParts[2].StartsWith("0"))
                                            {
                                                signal.MSB = true;
                                            }
                                            else
                                            {
                                                signal.MSB = false;
                                            }
                                            if (bParts[2].EndsWith("-"))
                                            {
                                                signal.Signed = true;
                                            }
                                            else
                                            {
                                                signal.Signed = false;
                                            }
                                            break;
                                        case 1: //Examples: (-0.005,0)   (1,1)
                                            string[] mParts = lParts[p].Split(new char[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
                                            if (mParts.Length != 2)
                                            {
                                                LoggerBold("Unknown conversion :" + lParts[p]);
                                                continue;
                                            }
                                            signal.Expression = "";
                                            int exprIdx = enumFuncs.IndexOf(signal.SignalName);
                                            if (exprIdx > -1)
                                            {
                                                signal.Expression = enumVals[exprIdx];
                                            }
                                            else
                                            {
                                                if (mParts[0] == "1")
                                                {
                                                    signal.Expression = "x";
                                                }
                                                else
                                                {
                                                    signal.Expression = mParts[0] + "*x";
                                                }
                                                if (mParts[1] != "0")
                                                {
                                                    if (mParts[1].StartsWith("-"))
                                                    {
                                                        signal.Expression += mParts[1];
                                                    }
                                                    else
                                                    {
                                                        signal.Expression += "+" + mParts[1];
                                                    }
                                                }
                                            }
                                            break;
                                        case 2:
                                            string[] aParts = lParts[p].Split(new char[] { '[', ']', '|' }, StringSplitOptions.RemoveEmptyEntries);
                                            if (aParts.Length != 2)
                                            {
                                                Debug.WriteLine("Unknown min & max: " + lParts[p]);
                                            }
                                            signal.Min = Convert.ToDouble(aParts[0], System.Globalization.CultureInfo.InvariantCulture);
                                            signal.Max = Convert.ToDouble(aParts[1], System.Globalization.CultureInfo.InvariantCulture);
                                            break;
                                        case 3:
                                            signal.Unit = lParts[p].Trim('"');
                                            break;
                                        case 4:
                                            signal.Receiver = lParts[p];
                                            break;
                                        default:
                                            Debug.WriteLine("Unknown token: " + lParts[p] + " in line: " + line + ", position: " + p.ToString());
                                            break;
                                    }
                                }
                                dbcMsg.Signals.Add(signal);
                            }
                        }
                    }
                    else if (line.StartsWith("CM_"))
                    {
                        string[] lParts = parseDBCrow(line);
                        if (lParts.Length > 3 && lParts[1] == "BU_")
                        {
                            List<DBCMsg> Msgs = retVal.Where(X => X.Sender == lParts[2]).ToList();
                            foreach(DBCMsg dMsg in Msgs)
                            {
                                dMsg.Description = lParts[3].Trim(';');
                            }
                        }
                        else if (lParts.Length > 4 && lParts[1] == "SG_")
                        {
                            DBCMsg dMsg = retVal.Where(X => X.CANid.ToString() == lParts[2]).FirstOrDefault();
                            if (dMsg != null)
                            {
                                DBCSignal dSignal = dMsg.Signals.Where(X => X.SignalName == lParts[3]).FirstOrDefault();
                                if (dSignal != null)
                                {
                                    dSignal.Description = lParts[4].Trim(';');
                                }
                            }

                        }
                    }
                    else if (line.StartsWith("VAL_TABLE"))
                    {
                        string[] lParts = parseDBCrow(line);
                        string expr = "enum: ";
                        for (int i=2;i<lParts.Length -1;i+=2)
                        {
                            expr += lParts[i] + ":" + lParts[i + 1].Replace(",","_").Replace(":","_");
                        }
                        enumFuncs.Add(lParts[1]);
                        enumVals.Add(expr);
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
                Debug.WriteLine("Error, DBC line " + line + ": " + ex.Message);
            }
            return retVal;
        }

        private static string[] parseDBCrow(string DbcRow)
        {
            List<string> retVal = new List<string>();
            StringBuilder rPart = new StringBuilder();
            int parCount = 0;
            for (int i = 0; i < DbcRow.Length; i++)
            {
                if (DbcRow[i] == '"')
                {
                    parCount++;
                }
                else if (DbcRow[i] == ' ' && parCount % 2 == 0 && rPart.ToString().Trim().Length > 0)
                {
                    retVal.Add(rPart.ToString().Trim());
                    rPart = new StringBuilder();
                    parCount = 0;
                }
                else if (DbcRow[i] == ' ' && parCount == 2)
                {
                    retVal.Add(rPart.ToString().Trim());
                    rPart = new StringBuilder();
                    parCount = 0;
                }
                else
                {
                    rPart.Append(DbcRow[i]);
                }
            }
            if (rPart.Length > 0)
                retVal.Add(rPart.ToString().Trim());
            return retVal.ToArray();
        }

        public static LogParam.ProfileDataType DBCToProfileDataType(DBCSignal signal)
        {
            int totalLen = signal.BitStartPos % 8 + signal.NumBits;
            if (totalLen <= 8)
            {
                if (signal.Signed)
                {
                    return LogParam.ProfileDataType.SBYTE;
                }
                else
                {
                    return LogParam.ProfileDataType.UBYTE;
                }
            }
            if (totalLen <= 16)
            {
                if (signal.Signed)
                {
                    return LogParam.ProfileDataType.SWORD;
                }
                else
                {
                    return LogParam.ProfileDataType.UWORD;
                }
            }
            if (totalLen <= 32)
            {
                if (signal.Signed)
                {
                    return LogParam.ProfileDataType.INT32;
                }
                else
                {
                    return LogParam.ProfileDataType.UINT32;
                }
            }
            return LogParam.ProfileDataType.UBYTE;
        }
        public static LogParam.PidParameter DBCtoParameter(DBCMsg msg, DBCSignal signal)
        {
            LogParam.PidParameter parm = new LogParam.PidParameter();
            parm.Type = LogParam.DefineBy.Passive;
            Conversion conversion = new Conversion(signal.Unit, signal.Expression, "0.00");
            parm.Conversions.Add(conversion);
            parm.Custom = true;
            parm.DataType = DBCToProfileDataType(signal);
            //parm.Id = "bdc"+signal.SignalName;
            parm.Name = signal.SignalName;
            parm.PassivePid.BitStartPos = signal.BitStartPos;
            parm.Address = (int)msg.CANid;
            parm.PassivePid.MsgLength = msg.DataLength;
            parm.PassivePid.NumBits = signal.NumBits;
            parm.PassivePid.MSB = signal.MSB;
            parm.PassivePid.Origin = msg.Sender;
            return parm;
        }
    }
}
