using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using static Helpers;
using static Upatcher;
using static LoggerUtils;
using System.Diagnostics;

namespace UniversalPatcher
{
    public class LogParamFile
    {
        public static void ImportPCMHammerFiles()
        {
            try
            {
                datalogger.PidParams.Clear();
                string fName = Path.Combine(Application.StartupPath, "Logger", "Parameters.Standard.xml");
                XDocument xml = XDocument.Load(fName);
                foreach (XElement parameterElement in xml.Root.Elements("Parameter"))
                {
                    LogParam.PidParameter parm = new LogParam.PidParameter();
                    parm.Id = parameterElement.Attribute("id").Value;
                    parm.Name = parameterElement.Attribute("name").Value;
                    parm.Description = parameterElement.Attribute("description").Value;
                    string DataType = parameterElement.Attribute("storageType").Value;
                    PidDataType pd = (PidDataType)Enum.Parse(typeof(PidDataType), DataType);
                    parm.DataType = (LogParam.ProfileDataType) ConvertToDataType(pd);
                    parm.Type = LogParam.DefineBy.Pid;
                    bool haveRaw = false;
                    foreach (XElement conversion in parameterElement.Elements("Conversion"))
                    {
                        string units = conversion.Attribute("units").Value;
                        if (units.ToLower() == "raw")
                        {
                            haveRaw = true;
                        }
                        string expression = conversion.Attribute("expression").Value;
                        string format = conversion.Attribute("format").Value;
                        if (parameterElement.Attribute("bitMapped").Value.ToLower() == "true")
                        {
                            int bitindex = Convert.ToInt32(parameterElement.Attribute("bitIndex").Value);
                            string[] truefalse = expression.Split(',');
                            parm.Conversions.Add(new Conversion(units, bitindex, truefalse[0], truefalse[1]));
                        }
                        else
                        {
                            parm.Conversions.Add(new Conversion(units, expression, format));
                        }
                    }
                    if (!haveRaw)
                    {
                        parm.Conversions.Add(new Conversion("Raw", "x", "0.00"));
                    }
                    datalogger.PidParams.Add(parm);
                }

                fName = Path.Combine(Application.StartupPath, "Logger", "Parameters.Math.xml");
                XDocument xmlMath = XDocument.Load(fName);
                foreach (XElement parameterElement in xmlMath.Root.Elements("MathParameter"))
                {
                    LogParam.PidParameter mathP = new LogParam.PidParameter();
                    mathP.Type = LogParam.DefineBy.Math;
                    mathP.Id = parameterElement.Attribute("id").Value;
                    mathP.Name = parameterElement.Attribute("name").Value;
                    mathP.Description = parameterElement.Attribute("description").Value;
                    string pid1Id = parameterElement.Attribute("xParameterId").Value;
                    string pid1Conversion = parameterElement.Attribute("xParameterConversion").Value;
                    mathP.AddPid(pid1Id, pid1Conversion);
                    if (parameterElement.Attribute("yParameterId") != null)
                    {
                        string pid2Id = parameterElement.Attribute("yParameterId").Value;
                        string pid2Conversion = parameterElement.Attribute("yParameterConversion").Value;
                        mathP.AddPid(pid2Id, pid2Conversion);
                    }
                    foreach (XElement conversion in parameterElement.Elements("Conversion"))
                    {
                        string units = conversion.Attribute("units").Value;
                        string expression = conversion.Attribute("expression").Value;
                        string format = conversion.Attribute("format").Value;
                        mathP.Conversions.Add(new Conversion(units, expression, format));
                    }
                    datalogger.PidParams.Add(mathP);
                }

                fName = Path.Combine(Application.StartupPath, "Logger", "Parameters.Ram.xml");
                XDocument xmlRam = XDocument.Load(fName);
                foreach (XElement parameterElement in xmlRam.Root.Elements("RamParameter"))
                {
                    LogParam.PidParameter ramP = new LogParam.PidParameter();
                    ramP.Type = LogParam.DefineBy.Address;
                    ramP.Id = parameterElement.Attribute("id").Value;
                    ramP.Name = parameterElement.Attribute("name").Value;
                    ramP.Description = parameterElement.Attribute("description").Value;
                    string DataType = parameterElement.Attribute("storageType").Value;
                    PidDataType pd = (PidDataType)Enum.Parse(typeof(PidDataType), DataType);
                    ramP.DataType = (LogParam.ProfileDataType)ConvertToDataType(pd);
                    bool haveRaw = false;
                    foreach (XElement conversion in parameterElement.Elements("Conversion"))
                    {
                        string units = conversion.Attribute("units").Value;
                        if (units.ToLower() == "raw")
                        {
                            haveRaw = true;
                        }
                        string expression = conversion.Attribute("expression").Value;
                        string format = conversion.Attribute("format").Value;
                        if (parameterElement.Attribute("bitMapped").Value.ToLower() == "true")
                        {
                            int bitindex = Convert.ToInt32(parameterElement.Attribute("bitIndex").Value);
                            string[] truefalse = expression.Split(',');
                            ramP.Conversions.Add(new Conversion(units, bitindex, truefalse[0], truefalse[1]));
                        }
                        else
                        {
                            ramP.Conversions.Add(new Conversion(units, expression, format));
                        }
                    }
                    if (!haveRaw)
                    {
                        ramP.Conversions.Add(new Conversion("Raw", "x", "0.00"));
                    }
                    foreach (XElement location in parameterElement.Elements("Location"))
                    {
                        LogParam.Location l = new LogParam.Location();
                        l.Os = location.Attribute("os").Value;
                        l.Address = location.Attribute("address").Value;
                        ramP.Locations.Add(l);
                    }
                    datalogger.PidParams.Add(ramP);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, LogParamFile line " + line + ": " + ex.Message);
            }
        }

        private static LogParam.PidParameter XmlToParameter(XElement parameterElement)
        {
            LogParam.PidParameter parm = new LogParam.PidParameter();
            try
            {
                string type = parameterElement.Attribute("Type").Value;
                parm.Type = (LogParam.DefineBy)Enum.Parse(typeof(LogParam.DefineBy), type);
                parm.Id = parameterElement.Attribute("id").Value;
                parm.Name = parameterElement.Attribute("name").Value;
                if (parameterElement.Attribute("description") != null)
                {
                    parm.Description = parameterElement.Attribute("description").Value;
                }
                if (parameterElement.Attribute("storageType") != null)
                {
                    string DataType = parameterElement.Attribute("storageType").Value;
                    PidDataType pd = (PidDataType)Enum.Parse(typeof(PidDataType), DataType);
                    parm.DataType = (LogParam.ProfileDataType)ConvertToDataType(pd);
                }
                if (parameterElement.Attribute("DataType") != null)
                {
                    string DataType = parameterElement.Attribute("DataType").Value;
                    parm.DataType = (LogParam.ProfileDataType)Enum.Parse(typeof(LogParam.ProfileDataType), DataType);
                }
                bool haveRaw = false;
                foreach (XElement conversion in parameterElement.Elements("Conversion"))
                {
                    string units = conversion.Attribute("units").Value;
                    if (units.ToLower() == "raw")
                    {
                        haveRaw = true;
                    }
                    string expression = conversion.Attribute("expression").Value;
                    string format = "0.00";
                    if (conversion.Attribute("format") != null)
                    {
                        format = conversion.Attribute("format").Value;
                    }
                    if (parameterElement.Attribute("bitmapped") != null && parameterElement.Attribute("bitmapped").Value.ToLower() == "true")
                    {
                        int bitindex = Convert.ToInt32(parameterElement.Attribute("bitIndex").Value);
                        string[] truefalse = expression.Split(',');
                        parm.Conversions.Add(new Conversion(units, bitindex, truefalse[0], truefalse[1]));
                    }
                    else
                    {
                        parm.Conversions.Add(new Conversion(units, expression, format));
                    }
                }
                switch (parm.Type)
                {
                    case LogParam.DefineBy.Address:
                        foreach (XElement location in parameterElement.Elements("Location"))
                        {
                            LogParam.Location l = new LogParam.Location();
                            if (location.Attribute("os") != null)
                            {
                                l.Os = location.Attribute("os").Value;
                            }
                            l.Address = location.Attribute("address").Value;
                            parm.Locations.Add(l);
                        }
                        break;
                    case LogParam.DefineBy.Math:
                        foreach (XElement lPid in parameterElement.Elements("Pid"))
                        {
                            string pidId = lPid.Attribute("id").Value;
                            string pidConversion = lPid.Attribute("conversion").Value;
                            string pidVar = lPid.Attribute("variable").Value;
                            parm.AddPid(pidId, pidConversion, pidVar);
                        }
                        break;
                    case LogParam.DefineBy.Passive:
                        if (parameterElement.Attribute("length") != null)
                        {
                            parm.PassivePid.MsgLength = Convert.ToInt32(parameterElement.Attribute("length").Value);
                        }
                        if (parameterElement.Attribute("position") != null)
                        {
                            parm.PassivePid.BitStartPos = Convert.ToInt32(parameterElement.Attribute("position").Value);
                        }
                        if (parameterElement.Attribute("numbits") != null)
                        {
                            parm.PassivePid.NumBits = Convert.ToInt32(parameterElement.Attribute("numbits").Value);
                        }
                        break;
                }
                if (!haveRaw)
                {
                    parm.Conversions.Add(new Conversion("Raw", "x", "0.00"));
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, LogParamFile line " + line + ": " + ex.Message);
            }
            return parm;
        }
        public static void LoadParamfile(string fName)
        {
            try
            {
                Logger("Loading file " + fName);
                datalogger.PidParams.Clear();
                XDocument xml = XDocument.Load(fName);
                foreach (XElement parameterElement in xml.Element("PidParams").Elements("Parameter"))
                {
                    LogParam.PidParameter parm = XmlToParameter(parameterElement);
                    datalogger.PidParams.Add(parm);
                }

                Logger("OK");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, LogParamFile line " + line + ": " + ex.Message);
            }
        }

        public static XElement ParameterToXml(LogParam.PidParameter parm)
        {
            XElement parameterElement = new XElement("Parameter");
            try
            {
                parameterElement.SetAttributeValue("id", parm.Id);
                parameterElement.SetAttributeValue("Type", parm.Type.ToString());
                parameterElement.SetAttributeValue("name", parm.Name);
                parameterElement.SetAttributeValue("description", parm.Description);
                parameterElement.SetAttributeValue("DataType", parm.DataType.ToString());
                switch (parm.Type)
                {
                    case LogParam.DefineBy.Pid:
                        parameterElement.SetAttributeValue("bitmapped", parm.Conversions[0].IsBitMapped.ToString());
                        parameterElement.SetAttributeValue("bitIndex", parm.Conversions[0].BitIndex);
                        break;
                    case LogParam.DefineBy.Address:
                        foreach (LogParam.Location location in parm.Locations)
                        {
                            XElement locationElement = new XElement("Location");
                            locationElement.SetAttributeValue("os", location.Os);
                            locationElement.SetAttributeValue("address", location.Address);
                            parameterElement.Add(locationElement);
                        }
                        break;
                    case LogParam.DefineBy.Math:
                        foreach (LogParam.LogPid pid in parm.Pids)
                        {
                            XElement pidElement = new XElement("Pid");
                            pidElement.SetAttributeValue("id", pid.Parameter.Id);
                            pidElement.SetAttributeValue("variable", pid.Variable);
                            pidElement.SetAttributeValue("conversion", pid.Units.Units);
                            if (pid.Os != null)
                            {
                                pidElement.SetAttributeValue("Os", pid.Os.Os);
                            }
                            parameterElement.Add(pidElement);
                        }
                        break;
                    case LogParam.DefineBy.Passive:
                        //parameterElement.SetAttributeValue("canid", parm.Address);
                        parameterElement.SetAttributeValue("length", parm.PassivePid.MsgLength);
                        parameterElement.SetAttributeValue("position", parm.PassivePid.BitStartPos);
                        parameterElement.SetAttributeValue("numbits", parm.PassivePid.NumBits);
                        break;
                }
                foreach (Conversion conversion in parm.Conversions)
                {
                    XElement conversionElement = new XElement("Conversion");
                    conversionElement.SetAttributeValue("units", conversion.Units);
                    if (parm.Conversions[0].IsBitMapped)
                    {
                        conversionElement.SetAttributeValue("expression", conversion.TrueValue + "," + conversion.FalseValue);
                    }
                    else
                    {
                        conversionElement.SetAttributeValue("expression", conversion.Expression);
                    }
                    conversionElement.SetAttributeValue("format", conversion.Format);
                    parameterElement.Add(conversionElement);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, LogParamFile line " + line + ": " + ex.Message);
            }
            return parameterElement;
        }
        public static void SaveParamfile(string fName)
        {
            try
            {
                Logger("Saving file " + fName);
                XElement xml = new XElement("PidParams");
                foreach (LogParam.PidParameter parm in datalogger.PidParams)
                {
                    if (!parm.Custom)
                    {
                        XElement parameterElement = ParameterToXml(parm);
                        xml.Add(parameterElement);
                    }
                }
                xml.Save(fName);
                Logger("OK");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, LogParamFile line " + line + ": " + ex.Message);
            }
        }

        private static void AddPidsToXml(LogParam.PidParameter parm, ref XElement xml)
        {
            foreach (LogParam.PidParameter pidParm in parm.GetLinkedPids())
            {
                XElement pidElement = ParameterToXml(pidParm);
                xml.Add(pidElement);
            }
        }
        public static void SaveProfile(string fName, List<LogParam.PidSettings> pids)
        {
            try
            {
                Logger("Saving profile: " + fName);
                XElement xml = new XElement("PidParams");
                foreach (LogParam.PidSettings pidSettings in pids)
                {
                    XElement parameterElement = ParameterToXml(pidSettings.Parameter);
                    XElement profileElement = new XElement("PidProfile");
                    if (pidSettings.Parameter.Type == LogParam.DefineBy.Math)
                    {
                        AddPidsToXml(pidSettings.Parameter, ref xml);
                    }
                    profileElement.SetAttributeValue("Units", pidSettings.Units.Units);
                    parameterElement.Add(profileElement);
                    xml.Add(parameterElement);
                }
                xml.Save(fName);
                Logger("OK");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, LogParamFile line " + line + ": " + ex.Message);
            }
        }

        private static LogParam.PidParameter FindParamFromMasterList(LogParam.PidParameter parm, ref bool SaveModifiedProfile)
        {
            List<LogParam.PidParameter> masterParms = datalogger.PidParams.Where(X => X.Name == parm.Name && X.Id == parm.Id).ToList();
            if (masterParms.Count == 0)
            {
                Logger("PID \"" + parm.Name + "\" not found from master list, adding as custom PID");
                parm.Custom = true;
                datalogger.PidParams.Add(parm);
                return parm;
            }
            bool isEqual = false;
            string parmXml = ParameterToXml(parm).ToString();
            foreach (LogParam.PidParameter masterParm in masterParms)
            {
                string masterXml = ParameterToXml(masterParm).ToString();
                if (parmXml == masterXml)
                {
                    isEqual = true;
                    break;
                }
            }
            if (!isEqual)
            {
                PidMisMatch misMatch = PidMisMatch.NewCustom;
                if (AppSettings.LoggerPidMismatchRemember)
                {
                    misMatch = AppSettings.LoggerPidMismatch;
                }
                else
                {
                    frmPidMismatch fpm = new frmPidMismatch();
                    fpm.labelMessage.Text = "Profile's PID settings differ from masterlist, please select method:";
                    fpm.labelMasterPid.Text = ParameterToXml(masterParms.Last()).ToString();
                    fpm.labelProfilePid.Text = ParameterToXml(parm).ToString();
                    if (fpm.ShowDialog() == DialogResult.OK)
                    {
                        if (fpm.radioNewCustom.Checked)
                        {
                            misMatch = PidMisMatch.NewCustom;
                        }
                        else if (fpm.radioUseMaster.Checked)
                        {
                            misMatch = PidMisMatch.UseMaster;
                        }
                        else if (fpm.radioUseMasterAndSave.Checked)
                        {
                            misMatch = PidMisMatch.UseMasterSave;
                        }
                        else
                        {
                            misMatch = PidMisMatch.UseProfile;
                        }
                        AppSettings.LoggerPidMismatchRemember = fpm.chkSaveSelection.Checked;
                        if (fpm.chkSaveSelection.Checked)
                        {
                            AppSettings.LoggerPidMismatch = misMatch;
                            AppSettings.LoggerPidMismatchRemember = true;
                        }
                        AppSettings.Save();
                    }
                    else
                    {
                        LoggerBold("Unknown selection!");
                        return null;
                    }
                }
                switch (misMatch)
                {
                    case PidMisMatch.NewCustom:
                        parm.Custom = true;
                        Logger("Adding new custom PID: \"" + parm.Name + "\"");
                        datalogger.PidParams.Add(parm);
                        break;
                    case PidMisMatch.UseMaster:
                        Logger("Using masterlist PID: \"" + parm.Name + "\"");
                        return masterParms.Last();
                    case PidMisMatch.UseMasterSave:
                        Logger("Using masterlist PID: \"" + parm.Name + "\"");
                        SaveModifiedProfile = true;
                        return masterParms.Last();
                    case PidMisMatch.UseProfile:
                        Logger("Updating masterlist PID \"" + parm.Name + "\"");
                        int idx = datalogger.PidParams.IndexOf(masterParms.Last());
                        datalogger.PidParams[idx] = parm;
                        return parm;
                }
            }
            return parm;
        }

        public static List<LogParam.PidSettings> LoadProfile(string fName)
        {
            List<LogParam.PidSettings> pids = new List<LogParam.PidSettings>();
            try
            {
                bool saveModifiedProfile = false;
                Logger("Loading profile: " + fName);
                if (fName.ToLower().EndsWith(".logprofile"))
                {
                    return ImportPcmhammerProfile(fName);                    
                }
                XDocument xml = XDocument.Load(fName);
                if (xml.Element("PidParams") == null && xml.Element("ArrayOfPidConfig") != null)
                {
                    Logger("Importing profile with old format");
                    return ImportOldProfile(fName);                    
                }
                if (xml.Element("PidParams") == null && xml.Element("ArrayOfPidProfile") != null)
                {
                    Logger("Importing profile with old format");
                    return ImportOldProfile2(fName);
                }
                if (xml.Element("PidParams") == null || xml.Element("PidParams").Elements("Parameter")== null)
                {
                    LoggerBold("Unknown profile format in file " + fName);
                    return null;
                }
                foreach (XElement parameterElement in xml.Element("PidParams").Elements("Parameter"))
                {
                    LogParam.PidParameter parm = XmlToParameter(parameterElement);
                    parm = FindParamFromMasterList(parm, ref saveModifiedProfile);
                    if (parameterElement.Elements("PidProfile") != null)
                    {
                        LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                        foreach (XElement profileElement in parameterElement.Elements("PidProfile"))
                        {
                            pidSettings.Units = parm.Conversions.Where(X => X.Units == profileElement.Attribute("Units").Value).FirstOrDefault();
                        }
                        pids.Add(pidSettings);
                    }
                }
                if (saveModifiedProfile)
                {
                    SaveProfile(fName, pids);
                }
                Logger("OK");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, LogParamFile line " + line + ": " + ex.Message);
            }
            return pids;
        }

        public static bool UpdateProfile(string fName, List<LogParam.PidParameter> parms)
        {
            bool needUpdate = false;
            List<LogParam.PidSettings> pids = new List<LogParam.PidSettings>();
            try
            {
                XDocument xml = XDocument.Load(fName);
                if (xml.Element("PidParams") == null && xml.Element("ArrayOfPidConfig") != null)
                {
                    Debug.WriteLine("Skipping old format profile: " + fName);
                    return false;
                }
                if (xml.Element("PidParams") == null || xml.Element("PidParams").Elements("Parameter") == null)
                {
                    Debug.WriteLine("Skipping unknown profile: " + fName);
                    return false;
                }
                foreach (XElement parameterElement in xml.Element("PidParams").Elements("Parameter"))
                {
                    LogParam.PidParameter parm = XmlToParameter(parameterElement);
                    if (parameterElement.Elements("PidProfile") != null)
                    {
                        LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                        XElement profileElement = parameterElement.Elements("PidProfile").FirstOrDefault();
                        if (profileElement != null)
                        {
                            pidSettings.Units = parm.Conversions.Where(X => X.Units == profileElement.Attribute("Units").Value).FirstOrDefault();
                            LogParam.PidParameter masterParm = parms.Where(X => X.Name == parm.Name && X.Id == parm.Id).LastOrDefault();
                            if (masterParm != null)
                            {
                                XElement masterXml = ParameterToXml(masterParm);
                                XElement profileXml = ParameterToXml(parm);
                                if (masterXml.ToString() != profileXml.ToString())
                                {
                                    needUpdate = true;
                                    pidSettings.Parameter = masterParm;
                                }
                            }
                            pids.Add(pidSettings);
                        }
                    }
                }
                if (needUpdate)
                {
                    SaveProfile(fName, pids);
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, LogParamFile line " + line + ": " + ex.Message);
            }
            return needUpdate;
        }

        public static List<LogParam.PidSettings> ImportOldProfile(string FileName)
        {
            List<LogParam.PidSettings> pids = new List<LogParam.PidSettings>();
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<PidConfig>));
                System.IO.StreamReader file = new System.IO.StreamReader(FileName);
                datalogger.PidProfile = (List<PidConfig>)reader.Deserialize(file);
                file.Close();
                foreach (PidConfig pc in datalogger.PidProfile)
                {
                    LogParam.PidParameter parm = datalogger.PidParams.Where(X => X.Name == pc.PidName).FirstOrDefault();
                    if (parm == null)
                    {
                        Logger("Can't import PID: " + pc.PidName);
                    }
                    else
                    {
                        LogParam.PidSettings newSettings = new LogParam.PidSettings(parm);
                        newSettings.Units = parm.Conversions.Where(X => X.Units == pc.Units).FirstOrDefault();
                        if (newSettings.Units == null)
                        {
                            newSettings.Units = parm.Conversions[0];
                        }
                        if (pc.Type == PidConfig.DefineBy.Address)
                        {
                            newSettings.Os = parm.Locations.Where(X => X.Address.Contains(pc.Address)).FirstOrDefault();
                        }
                        pids.Add(newSettings);
                    }
                }
                Logger("OK");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Datalogger line " + line + ": " + ex.Message);
            }
            return pids;
        }
        public static List<LogParam.PidSettings> ImportOldProfile2(string FileName)
        {
            List<LogParam.PidSettings> pids = new List<LogParam.PidSettings>();
            List<LogParam.PidProfile> profiles = new List<LogParam.PidProfile>();
            try
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<LogParam.PidProfile>));
                System.IO.StreamReader file = new System.IO.StreamReader(FileName);
                profiles = (List<LogParam.PidProfile>)reader.Deserialize(file);
                file.Close();
                foreach (LogParam.PidProfile pp in profiles)
                {
                    LogParam.PidParameter parm = datalogger.PidParams.Where(X => X.Name == pp.Id).FirstOrDefault();
                    if (parm == null)
                    {
                        Logger("Can't import PID: " + pp.Id);
                    }
                    else
                    {
                        LogParam.PidSettings newSettings = new LogParam.PidSettings(parm);
                        newSettings.Units = parm.Conversions.Where(X => X.Units == pp.Conversion).FirstOrDefault();
                        if (newSettings.Units == null)
                        {
                            newSettings.Units = parm.Conversions[0];
                        }
                        pids.Add(newSettings);
                    }
                }
                Logger("OK");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Datalogger line " + line + ": " + ex.Message);
            }
            return pids;
        }

        private static List<LogParam.PidSettings> ImportPcmhammerProfile(string fName)
        {
            List<LogParam.PidSettings> pids = new List<LogParam.PidSettings>();
            try
            {
                Logger("Importing PCM Hammer profile");
                XDocument xml = XDocument.Load(fName);
                foreach (XElement parameterElement in xml.Elements("LogProfile").Elements("PidParameters").Elements("PidParameter"))
                {
                    string id = parameterElement.Attribute("id").Value;
                    LogParam.PidParameter parm = datalogger.PidParams.Where(X => X.Name.Replace(" ", "").Replace("/", "") == id).FirstOrDefault();
                    if (parm != null)
                    {
                        LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                        if (parameterElement.Attribute("units") != null)
                        {
                            pidSettings.Units = parm.Conversions.Where(X => X.Units == parameterElement.Attribute("units").Value).FirstOrDefault();
                        }
                        pids.Add(pidSettings);
                    }
                    else
                    {
                        LoggerBold("Skipping unknown PID: " + id);
                    }

                }
                foreach (XElement parameterElement in xml.Elements("LogProfile").Elements("RamParameters").Elements("RamParameter"))
                {
                    string id = parameterElement.Attribute("id").Value;
                    LogParam.PidParameter parm = datalogger.PidParams.Where(X => X.Id.Replace(" ", "").Replace("/", "") == id).FirstOrDefault();
                    if (parm != null)
                    {
                        LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                        if (parameterElement.Attribute("units") != null)
                        {
                            pidSettings.Units = parm.Conversions.Where(X => X.Units == parameterElement.Attribute("units").Value).FirstOrDefault();
                        }
                        pids.Add(pidSettings);
                    }
                    else
                    {
                        LoggerBold("Skipping unknown PID: " + id);
                    }

                }
                foreach (XElement parameterElement in xml.Elements("LogProfile").Elements("MathParameters").Elements("MathParameter"))
                {
                    string id = parameterElement.Attribute("id").Value;
                    LogParam.PidParameter parm = datalogger.PidParams.Where(X => X.Id.Replace(" ", "").Replace("/", "") == id).FirstOrDefault();
                    if (parm != null)
                    {
                        LogParam.PidSettings pidSettings = new LogParam.PidSettings(parm);
                        if (parameterElement.Attribute("units") != null)
                        {
                            pidSettings.Units = parm.Conversions.Where(X => X.Units == parameterElement.Attribute("units").Value).FirstOrDefault();
                        }
                        pids.Add(pidSettings);
                    }
                    else
                    {
                        LoggerBold("Skipping unknown PID: " + id);
                    }

                }
                Logger("OK");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, Datalogger line " + line + ": " + ex.Message);
            }
            return pids;
        }


    }
}
