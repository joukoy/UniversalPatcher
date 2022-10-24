using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using UniversalPatcher;
using static Upatcher;

public static class Helpers
{
    public static string BinFilter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
    public static string CsvFilter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
    public static string XmlFilter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
    public static string TxtFilter = "TXT files (*.txt)|*.txt|All files (*.*)|*.*";
    public static string XdfFilter = "XDF files (*.xdf)|*.xdf|All files (*.*)|*.*";
    public static string RtfFilter = "RTF files (*.rtf)|*.rtf|All files (*.*)|*.*";
    public static string RtfFTxtilter = "RTF or TXT (*.rtf;*.txt)|*.rtf;*.txt|All files (*.*)|*.*";

    public static UPLogger uPLogger = new UPLogger();
    public static DataLogger.LogDataEvents LoggerDataEvents = new DataLogger.LogDataEvents();

    public static void Logger(string LogText, Boolean NewLine = true, bool Bold= false)
    {
        uPLogger.Add(LogText, NewLine, Bold);
    }
    public static void LoggerBold(string LogText, Boolean NewLine = true)
    {
        Logger(LogText, NewLine, true);
    }


    public static byte[] ReadBin(string FileName)
    {

        uint Length = (uint)new FileInfo(FileName).Length;
        byte[] buf = new byte[Length];

        long offset = 0;
        long remaining = Length;

        using (BinaryReader freader = new BinaryReader(File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
        {
            freader.BaseStream.Seek(0, 0);
            while (remaining > 0)
            {
                int read = freader.Read(buf, (int)offset, (int)remaining);
                if (read <= 0)
                    throw new EndOfStreamException
                        (String.Format("End of stream reached with {0} bytes left to read", remaining));
                remaining -= read;
                offset += read;
            }
            freader.Close();
        }
        return buf;
    }

    public static void WriteBinToFile(string FileName, byte[] Buf)
    {
        using (FileStream stream = new FileStream(FileName, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Buf);
                writer.Close();
            }
        }
    }

    public static void WriteSegmentToFile(string FileName, List<Block> Addr, byte[] Buf)
    {
        using (FileStream stream = new FileStream(FileName, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                for (int b = 0; b < Addr.Count; b++)
                {
                    uint StartAddr = Addr[b].Start;
                    uint Length = Addr[b].End - Addr[b].Start + 1;
                    writer.Write(Buf, (int)StartAddr, (int)Length);
                }
                writer.Close();
            }
        }
    }

    public static string ReadTextFile(string fileName)
    {
        StreamReader sr = new StreamReader(fileName);
        string fileContent = sr.ReadToEnd();
        sr.Close();
        return fileContent;
    }

    public static void WriteTextFile(string fileName, string fileContent, bool append = false)
    {
        using (StreamWriter writetext = new StreamWriter(fileName, append))
        {
            writetext.Write(fileContent);
        }
    }

    public static List<string> SelectMultipleFiles(string Title = "Select files", string Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*", string defaultFile = "")
    {
        List<string> fileList = new List<string>();

        OpenFileDialog fdlg = new OpenFileDialog();
        if (Filter.Contains("BIN"))
        {
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;
            Filter = "BIN files (*.bin)|*.bin";
            for (int f = 0; f < fileTypeList.Count; f++)
            {
                string newFilter = "|" + fileTypeList[f].Description + "|" + "*." + fileTypeList[f].Extension;
                Filter += newFilter;
            }
            Filter += "|All files (*.*)|*.*";
        }
        else if (Filter.ToLower().Contains("xdf"))
        {
            fdlg.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tunerpro Files", "Bin Definitions");
        }
        else if (defaultFile.Length > 0)
        {

            fdlg.FileName = Path.GetFileName(defaultFile);
            fdlg.InitialDirectory = Path.GetDirectoryName(defaultFile);
        }
        else
        {
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastXMLfolder;
            if (Filter.Contains("PATCH") || Filter.Contains("TXT"))
                fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastPATCHfolder;
        }

        fdlg.Title = Title;
        fdlg.Filter = Filter;
        fdlg.FilterIndex = 1;
        fdlg.RestoreDirectory = true;
        fdlg.Multiselect = true;
        if (fdlg.ShowDialog() == DialogResult.OK)
        {
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastXMLfolder = Path.GetDirectoryName(fdlg.FileName);
            else if (Filter.Contains("BIN"))
                UniversalPatcher.Properties.Settings.Default.LastBINfolder = Path.GetDirectoryName(fdlg.FileName);
            else if (Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastPATCHfolder = Path.GetDirectoryName(fdlg.FileName);
            UniversalPatcher.Properties.Settings.Default.Save();
            foreach (string fName in fdlg.FileNames)
                fileList.Add(fName);
        }
        return fileList;
    }

    private static string GenerateFilter()
    {
        string Filter = "BIN files (*.bin)|*.bin";
        int def = int.MaxValue;
        for (int f = 0; f < fileTypeList.Count; f++)
        {
            if (fileTypeList[f].Default)
                def = f;
        }

        if (def < int.MaxValue)
        {
            Filter = fileTypeList[def].Description + "|" + fileTypeList[def].Extension;
        }
        for (int f = 0; f < fileTypeList.Count; f++)
        {
            if (f != def)
            {
                string newFilter = "|" + fileTypeList[f].Description + "|" + fileTypeList[f].Extension;
                Filter += newFilter;
            }
        }
        Filter += "|All files (*.*)|*.*";
        return Filter;
    }

    public static string SelectFile(string Title = "Select file", string Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*", string defaultFile = "")
    {
        OpenFileDialog fdlg = new OpenFileDialog();
        if (Filter.Contains("BIN"))
        {
            fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;
            Filter = GenerateFilter();
        }
        else if (Filter.ToLower().Contains("xdf"))
        {
            fdlg.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tunerpro Files", "Bin Definitions");
        }
        else if (defaultFile.Length > 0)
        {

            fdlg.FileName = Path.GetFileName(defaultFile);
            fdlg.InitialDirectory = Path.GetDirectoryName(defaultFile);
        }
        else
        {
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastXMLfolder;
            if (Filter.Contains("PATCH") )
                fdlg.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastPATCHfolder;
        }

        fdlg.Title = Title;
        fdlg.Filter = Filter;
        fdlg.FilterIndex = 1;
        fdlg.RestoreDirectory = true;

        if (fdlg.ShowDialog() == DialogResult.OK)
        {
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastXMLfolder = Path.GetDirectoryName(fdlg.FileName);
            else if (Filter.Contains("BIN"))
                UniversalPatcher.Properties.Settings.Default.LastBINfolder = Path.GetDirectoryName(fdlg.FileName);
            else if (Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastPATCHfolder = Path.GetDirectoryName(fdlg.FileName);
            UniversalPatcher.Properties.Settings.Default.Save();
            return fdlg.FileName;
        }
        return "";
    }

    public static string SelectSaveFile(string Filter = "", string defaultFileName = "")
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        //saveFileDialog.Filter = "BIN files (*.bin)|*.bin";
        if (Filter == "" || Filter.Contains("BIN"))
            //saveFileDialog.Filter = "BIN files (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog.Filter = GenerateFilter();
        else
            saveFileDialog.Filter = Filter;
        saveFileDialog.RestoreDirectory = true;
        saveFileDialog.Title = "Save to file";
        if (defaultFileName.Length > 0)
        {
            saveFileDialog.FileName = Path.GetFileName(defaultFileName);
            string defPath = Path.GetDirectoryName(defaultFileName);
            if (defPath != "")
            {
                saveFileDialog.InitialDirectory = defPath;
            }
        }
        else
        {
            if (Filter.Contains("PATCH"))
                saveFileDialog.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastPATCHfolder;
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                saveFileDialog.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastXMLfolder;
            else if (Filter.Contains("BIN"))
                saveFileDialog.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;
            else if (Filter.Contains("XDF"))
                saveFileDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Tunerpro Files", "Bin Definitions");
        }

        if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            if (Filter.Contains("XML") && !Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastXMLfolder = Path.GetDirectoryName(saveFileDialog.FileName);
            else if (Filter.Contains("BIN"))
                UniversalPatcher.Properties.Settings.Default.LastBINfolder = Path.GetDirectoryName(saveFileDialog.FileName);
            else if (Filter.Contains("PATCH"))
                UniversalPatcher.Properties.Settings.Default.LastPATCHfolder = Path.GetDirectoryName(saveFileDialog.FileName);
            UniversalPatcher.Properties.Settings.Default.Save();
            return saveFileDialog.FileName;
        }
        else
            return "";
    }

    public static string SelectFolder(string Title, string defaultFolder = "")
    {
        string folderPath = "";
        OpenFileDialog folderBrowser = new OpenFileDialog();
        // Set validate names and check file exists to false otherwise windows will
        // not let you select "Folder Selection."
        folderBrowser.ValidateNames = false;
        folderBrowser.CheckFileExists = false;
        folderBrowser.CheckPathExists = true;
        if (defaultFolder.Length > 0)
            folderBrowser.InitialDirectory = defaultFolder;
        else
            folderBrowser.InitialDirectory = UniversalPatcher.Properties.Settings.Default.LastBINfolder;
        // Always default to Folder Selection.
        folderBrowser.Title = Title;
        folderBrowser.FileName = "Folder Selection";
        if (folderBrowser.ShowDialog() == DialogResult.OK)
        {
            folderPath = Path.GetDirectoryName(folderBrowser.FileName);
            UniversalPatcher.Properties.Settings.Default.LastBINfolder = folderPath;
            UniversalPatcher.Properties.Settings.Default.Save();
        }
        return folderPath;
    }

    [DllImport("shell32.dll", SetLastError = true)]
    public static extern int SHOpenFolderAndSelectItems(IntPtr pidlFolder, uint cidl, [In, MarshalAs(UnmanagedType.LPArray)] IntPtr[] apidl, uint dwFlags);

    [DllImport("shell32.dll", SetLastError = true)]
    public static extern void SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string name, IntPtr bindingContext, [Out] out IntPtr pidl, uint sfgaoIn, [Out] out uint psfgaoOut);

    public static void OpenFolderAndSelectItem(string folderPath, string file)
    {
        IntPtr nativeFolder;
        uint psfgaoOut;
        SHParseDisplayName(folderPath, IntPtr.Zero, out nativeFolder, 0, out psfgaoOut);

        if (nativeFolder == IntPtr.Zero)
        {
            // Log error, can't find folder
            return;
        }

        IntPtr nativeFile;
        SHParseDisplayName(Path.Combine(folderPath, file), IntPtr.Zero, out nativeFile, 0, out psfgaoOut);

        IntPtr[] fileArray;
        if (nativeFile == IntPtr.Zero)
        {
            // Open the folder without the file selected if we can't find the file
            fileArray = new IntPtr[0];
        }
        else
        {
            fileArray = new IntPtr[] { nativeFile };
        }

        SHOpenFolderAndSelectItems(nativeFolder, (uint)fileArray.Length, fileArray, 0);

        Marshal.FreeCoTaskMem(nativeFolder);
        if (nativeFile != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(nativeFile);
        }
    }

    public static bool HexToUint64(string Hex, out UInt64 x)
    {
        x = 0;
        if (!UInt64.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out x))
            return false;
        return true;
    }

    public static bool HexToUint(string Hex, out uint x)
    {
        x = 0;
        if (!UInt32.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out x))
            return false;
        return true;
    }
    public static bool HexToInt(string Hex, out int x)
    {
        x = 0;
        if (!int.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out x))
            return false;
        return true;
    }

    public static bool HexToUshort(string Hex, out ushort x)
    {
        x = 0;
        if (!UInt16.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out x))
            return false;
        return true;
    }
    public static bool HexToByte(string Hex, out byte x)
    {
        x = 0;
        if (!byte.TryParse(Hex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out x))
            return false;
        return true;
    }

    public static string ReadTextBlock(byte[] buf, int Address, int Bytes, bool numsLettersOnly = true)
    {
        string result = System.Text.Encoding.ASCII.GetString(buf, (int)Address, Bytes);
        if (numsLettersOnly)
            result = Regex.Replace(result, "[^a-zA-Z0-9]", "?");
        else
            result = Regex.Replace(result, @"[^\u0020-\u007E]", "?");
        return result;
    }

    public static UInt64 ReadUint64(byte[] buf, uint offset, bool MSB)
    {
        byte[] tmp = new byte[8];
        Array.Copy(buf, offset, tmp, 0, 8);
        if (MSB)
            Array.Reverse(tmp);
        return BitConverter.ToUInt64(tmp, 0);
    }

    public static Int64 ReadInt64(byte[] buf, uint offset, bool MSB)
    {
        byte[] tmp = new byte[8];
        Array.Copy(buf, offset, tmp, 0, 8);
        if (MSB)
            Array.Reverse(tmp);
        return BitConverter.ToInt64(tmp, 0);
    }
    public static Double ReadFloat64(byte[] buf, uint offset, bool MSB)
    {
        byte[] tmp = new byte[8];
        Array.Copy(buf, offset, tmp, 0, 8);
        if (MSB)
            Array.Reverse(tmp);
        return BitConverter.ToDouble(tmp, 0);
    }
    public static float ReadFloat32(byte[] buf, uint offset, bool MSB)
    {
        byte[] tmp = new byte[4];
        Array.Copy(buf, offset, tmp, 0, 4);
        if (MSB)
            Array.Reverse(tmp);
        return BitConverter.ToSingle(tmp, 0);
    }

    public static uint ReadUint32(byte[] buf, uint offset, bool MSB)
    {
        //Shift first byte 24 bits left, second 16bits left...
        //return (uint)((buf[offset] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3]);
        byte[] tmp = new byte[4];
        Array.Copy(buf, offset, tmp, 0, 4);
        if (MSB)
            Array.Reverse(tmp);
        return BitConverter.ToUInt32(tmp, 0);
    }

    public static UInt16 ReadUint16(byte[] buf, uint offset, bool MSB)
    {
        if (MSB)
            return (UInt16)((buf[offset] << 8) | buf[offset + 1]);
        else
            return (UInt16)((buf[offset + 1] << 8) | buf[offset]);
    }

    public static int ReadInt32(byte[] buf, uint offset, bool MSB)
    {
        //Shift first byte 24 bits left, second 16bits left...
        //return (int)((buf[offset] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3]);
        byte[] tmp = new byte[4];
        Array.Copy(buf, offset, tmp, 0, 4);
        if (MSB)
            Array.Reverse(tmp);
        return BitConverter.ToInt32(tmp, 0);
    }

    public static Int16 ReadInt16(byte[] buf, uint offset, bool MSB)
    {
        if (MSB)
            return (Int16)((buf[offset] << 8) | buf[offset + 1]);
        else
            return (Int16)((buf[offset + 1] << 8) | buf[offset]);
    }
    public static void SaveFloat32(byte[] buf, uint offset, Single data, bool MSB)
    {
        byte[] tmp = new byte[4];
        tmp = BitConverter.GetBytes(data);
        if (MSB)
            Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 4);
    }
    public static void SaveFloat64(byte[] buf, uint offset, double data, bool MSB)
    {
        byte[] tmp = new byte[8];
        tmp = BitConverter.GetBytes(data);
        if (MSB)
            Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 8);
    }

    public static void SaveUint64(byte[] buf, uint offset, UInt64 data, bool MSB)
    {
        byte[] tmp = new byte[8];
        tmp = BitConverter.GetBytes(data);
        if (MSB)
            Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 8);
    }

    public static void SaveInt64(byte[] buf, uint offset, Int64 data, bool MSB)
    {
        byte[] tmp = new byte[8];
        tmp = BitConverter.GetBytes(data);
        if (MSB)
            Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 8);
    }
    public static void SaveUint32(byte[] buf, uint offset, UInt32 data, bool MSB)
    {
        byte[] tmp = new byte[4];
        tmp = BitConverter.GetBytes(data);
        if (MSB)
            Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 4);
    }
    public static void SaveInt32(byte[] buf, uint offset, Int32 data, bool MSB)
    {
        byte[] tmp = new byte[4];
        tmp = BitConverter.GetBytes(data);
        if (MSB)
            Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 4);
    }

    public static void Save3Bytes(byte[] buf, uint offset, UInt32 data, bool MSB)
    {
        if (MSB)
        {
            buf[offset] = (byte)(data & 0xff);
            buf[offset + 1] = (byte)((data >> 8) & 0xff);
            buf[offset + 2] = (byte)((data >> 16) & 0xff);
        }
        else
        {
            buf[offset + 2] = (byte)(data & 0xff);
            buf[offset + 1] = (byte)((data >> 8) & 0xff);
            buf[offset] = (byte)((data >> 16) & 0xff);
        }
    }

    public static void SaveUshort(byte[] buf, uint offset, ushort data, bool MSB)
    {
        byte[] tmp = new byte[2];
        tmp = BitConverter.GetBytes(data);
        if (MSB)
            Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 2);
    }
    public static void SaveShort(byte[] buf, uint offset, short data, bool MSB)
    {
        byte[] tmp = new byte[2];
        tmp = BitConverter.GetBytes(data);
        if (MSB)
            Array.Reverse(tmp);
        Array.Copy(tmp, 0, buf, offset, 2);
    }

    /*    public static ushort SwapBytes(ushort x)
        {
            return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
        }

        public static uint SwapBytes(uint x)
        {
            return ((x & 0x000000ff) << 24) +
                    ((x & 0x0000ff00) << 8) +
                    ((x & 0x00ff0000) >> 8) +
                    ((x & 0xff000000) >> 24);
        }
    */
    public static UInt64 SwapBytes(UInt64 data, int bytes)
    {
        byte[] tmp = new byte[8];
        tmp = BitConverter.GetBytes(data);
        byte[] tmp2 = new byte[bytes];
        Array.Copy(tmp, 0, tmp2, 0, bytes);
        Array.Reverse(tmp2);
        tmp = BitConverter.GetBytes((UInt64)0);
        Array.Copy(tmp2, tmp, bytes);
        return BitConverter.ToUInt64(tmp, 0);
    }

    public static void UseComboBoxForEnums(DataGridView g)
    {
        try
        {
            g.Columns.Cast<DataGridViewColumn>()
                .Where(x => x.ValueType.IsEnum && x.GetType() != typeof(DataGridViewComboBoxColumn))
                .ToList().ForEach(x =>
                {
                    var index = x.Index;
                    g.Columns.RemoveAt(index);
                    var c = new DataGridViewComboBoxColumn();
                    c.ValueType = x.ValueType;
                    c.ValueMember = "Value";
                    c.DisplayMember = "Name";
                    c.DataPropertyName = x.DataPropertyName;
                    c.HeaderText = x.HeaderText;
                    c.Name = x.Name;
                    if (x.ValueType.IsEnum)
                    {
                        c.DataSource = Enum.GetValues(x.ValueType).Cast<object>().Select(v => new
                        {
                            Value = (int)v,
                            Name = Enum.GetName(x.ValueType, v) /* or any other logic to get text */
                        }).ToList();
                    }

                    g.Columns.Insert(index, c);
                });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }

    public static string GetShortcutTarget(string shortcutFilename)
    {
        try
        {
            string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            Shell32.Shell shell = new Shell32.Shell();
            Shell32.Folder folder = shell.NameSpace(pathOnly);
            Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                return link.Path;
            }
            return ""; // not found
        }
        catch
        {
            return "";
        }
    }


    public static void CreateShortcut(string DestinationFolder, string scName, string args)
    {
        object shDesktop = (object)"Desktop";

        IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
        string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop);
        if (DestinationFolder.Length > 0)
            shortcutAddress = DestinationFolder;
        shortcutAddress = Path.Combine(shortcutAddress, scName + ".lnk");
        IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutAddress);
        shortcut.Description = "UniversalPatcher";
        shortcut.Arguments = args;
        shortcut.TargetPath = Application.ExecutablePath;
        shortcut.Save();
    }

    private static string GetNextBase26(string a)
    {
        return Base26Sequence().SkipWhile(x => x != a).Skip(1).First();
    }

    private static IEnumerable<string> Base26Sequence()
    {
        long i = 0L;
        while (true)
            yield return Base26Encode(i++);
    }

    private static char[] base26Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    public static string Base26Encode(Int64 value)
    {
        string returnValue = null;
        do
        {
            returnValue = base26Chars[value % 26] + returnValue;
            value /= 26;
        } while (value-- != 0);
        return returnValue;
    }


}
