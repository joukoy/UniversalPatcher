using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using static upatcher;
using System.Diagnostics;

namespace UniversalPatcher
{
    public partial class frmTuner : Form
    {
        public frmTuner(PcmFile PCM1)
        {
            InitializeComponent();
            PCM = PCM1;
            if (upatcher.Segments[0].CS1Address.StartsWith("GM-V6"))
                btnReadTinyTunerDB.Enabled = true;
            else
                btnReadTinyTunerDB.Enabled = false;
        }

        private PcmFile PCM;
        
        BindingSource bindingsource = new BindingSource();
        BindingSource categoryBindingSource = new BindingSource();
        private BindingList<TableData> filteredCategories = new BindingList<TableData>();
        private void btnEditTable_Click(object sender, EventArgs e)
        {
            try
            {

                int rowindex = dataGridView1.CurrentCell.RowIndex;
                int columnindex = dataGridView1.CurrentCell.ColumnIndex;
                int codeIndex = Convert.ToInt32(dataGridView1.Rows[rowindex].Cells["id"].Value);
                frmTableEditor frmT = new frmTableEditor();
                TableData td = tableDatas[codeIndex];
                frmT.loadTable(td, PCM);
                if ((frmT.ShowDialog()) == DialogResult.OK)
                {
                    LoggerBold("File modified, you can now save it");
                }
                frmT.Dispose();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }
        public void LoggerBold(string LogText, Boolean NewLine = true)
        {
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Bold);
            txtResult.AppendText(LogText);
            txtResult.SelectionFont = new Font(txtResult.Font, FontStyle.Regular);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
        }

        public void Logger(string LogText, Boolean NewLine = true)
        {
            int Start = txtResult.Text.Length;
            txtResult.AppendText(LogText);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
        }
        private bool FixCheckSums()
        {
            bool NeedFix = false;
            try
            {
                Logger("Fixing Checksums:");
                for (int i = 0; i < Segments.Count; i++)
                {
                    SegmentConfig S = Segments[i];
                    Logger(S.Name);
                    if (S.Eeprom)
                    {
                        string Ret = GmEeprom.FixEepromKey(PCM.buf);
                        if (Ret.Contains("Fixed"))
                            NeedFix = true;
                        Logger(Ret);
                    }
                    else
                    {
                        if (S.CS1Method != CSMethod_None)
                        {
                            uint CS1 = 0;
                            uint CS1Calc = CalculateChecksum(PCM.buf, PCM.binfile[i].CS1Address, PCM.binfile[i].CS1Blocks, PCM.binfile[i].ExcludeBlocks, S.CS1Method, S.CS1Complement, PCM.binfile[i].CS1Address.Bytes, S.CS1SwapBytes);
                            if (PCM.binfile[i].CS1Address.Address < uint.MaxValue)
                            {
                                if (PCM.binfile[i].CS1Address.Bytes == 1)
                                {
                                    CS1 = PCM.buf[PCM.binfile[i].CS1Address.Address];
                                }
                                else if (PCM.binfile[i].CS1Address.Bytes == 2)
                                {
                                    CS1 = BEToUint16(PCM.buf, PCM.binfile[i].CS1Address.Address);
                                }
                                else if (PCM.binfile[i].CS1Address.Bytes == 4)
                                {
                                    CS1 = BEToUint32(PCM.buf, PCM.binfile[i].CS1Address.Address);
                                }
                            }
                            if (CS1 == CS1Calc)
                                Logger(" Checksum 1: " + CS1.ToString("X4") + " [OK]");
                            else
                            {
                                if (PCM.binfile[i].CS1Address.Address == uint.MaxValue)
                                {
                                    string hexdigits;
                                    if (PCM.binfile[i].CS1Address.Bytes == 0)
                                        hexdigits = "X4";
                                    else
                                        hexdigits = "X" + (PCM.binfile[i].CS1Address.Bytes * 2).ToString();
                                    Logger(" Checksum 1: " + CS1Calc.ToString(hexdigits) + " [Not saved]");
                                }
                                else
                                {
                                    if (PCM.binfile[i].CS1Address.Bytes == 1)
                                        PCM.buf[PCM.binfile[i].CS1Address.Address] = (byte)CS1Calc;
                                    else if (PCM.binfile[i].CS1Address.Bytes == 2)
                                    {
                                        PCM.buf[PCM.binfile[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF00) >> 8);
                                        PCM.buf[PCM.binfile[i].CS1Address.Address + 1] = (byte)(CS1Calc & 0xFF);
                                    }
                                    else if (PCM.binfile[i].CS1Address.Bytes == 4)
                                    {
                                        PCM.buf[PCM.binfile[i].CS1Address.Address] = (byte)((CS1Calc & 0xFF000000) >> 24);
                                        PCM.buf[PCM.binfile[i].CS1Address.Address + 1] = (byte)((CS1Calc & 0xFF0000) >> 16);
                                        PCM.buf[PCM.binfile[i].CS1Address.Address + 2] = (byte)((CS1Calc & 0xFF00) >> 8);
                                        PCM.buf[PCM.binfile[i].CS1Address.Address + 3] = (byte)(CS1Calc & 0xFF);

                                    }
                                    Logger(" Checksum 1: " + CS1.ToString("X") + " => " + CS1Calc.ToString("X4") + " [Fixed]");
                                    NeedFix = true;
                                }
                            }
                        }

                        if (S.CS2Method != CSMethod_None)
                        {
                            uint CS2 = 0;
                            uint CS2Calc = CalculateChecksum(PCM.buf, PCM.binfile[i].CS2Address, PCM.binfile[i].CS2Blocks, PCM.binfile[i].ExcludeBlocks, S.CS2Method, S.CS2Complement, PCM.binfile[i].CS2Address.Bytes, S.CS2SwapBytes);
                            if (PCM.binfile[i].CS2Address.Address < uint.MaxValue)
                            {
                                if (PCM.binfile[i].CS2Address.Bytes == 1)
                                {
                                    CS2 = PCM.buf[PCM.binfile[i].CS2Address.Address];
                                }
                                else if (PCM.binfile[i].CS2Address.Bytes == 2)
                                {
                                    CS2 = BEToUint16(PCM.buf, PCM.binfile[i].CS2Address.Address);
                                }
                                else if (PCM.binfile[i].CS2Address.Bytes == 4)
                                {
                                    CS2 = BEToUint32(PCM.buf, PCM.binfile[i].CS2Address.Address);
                                }
                            }
                            if (CS2 == CS2Calc)
                                Logger(" Checksum 2: " + CS2.ToString("X4") + " [OK]");
                            else
                            {
                                if (PCM.binfile[i].CS2Address.Address == uint.MaxValue)
                                {
                                    string hexdigits;
                                    if (PCM.binfile[i].CS1Address.Bytes == 0)
                                        hexdigits = "X4";
                                    else
                                        hexdigits = "X" + (PCM.binfile[i].CS2Address.Bytes * 2).ToString();
                                    Logger(" Checksum 2: " + CS2Calc.ToString("X4") + " [Not saved]");
                                }
                                else
                                {
                                    if (PCM.binfile[i].CS2Address.Bytes == 1)
                                        PCM.buf[PCM.binfile[i].CS2Address.Address] = (byte)CS2Calc;
                                    else if (PCM.binfile[i].CS2Address.Bytes == 2)
                                    {
                                        PCM.buf[PCM.binfile[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF00) >> 8);
                                        PCM.buf[PCM.binfile[i].CS2Address.Address + 1] = (byte)(CS2Calc & 0xFF);
                                    }
                                    else if (PCM.binfile[i].CS2Address.Bytes == 4)
                                    {
                                        PCM.buf[PCM.binfile[i].CS2Address.Address] = (byte)((CS2Calc & 0xFF000000) >> 24);
                                        PCM.buf[PCM.binfile[i].CS2Address.Address + 1] = (byte)((CS2Calc & 0xFF0000) >> 16);
                                        PCM.buf[PCM.binfile[i].CS2Address.Address + 2] = (byte)((CS2Calc & 0xFF00) >> 8);
                                        PCM.buf[PCM.binfile[i].CS2Address.Address + 3] = (byte)(CS2Calc & 0xFF);

                                    }
                                    Logger(" Checksum 2: " + CS2.ToString("X") + " => " + CS2Calc.ToString("X4") + " [Fixed]");
                                    NeedFix = true;
                                }
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }
            return NeedFix;
        }

        private void btnSaveBin_Click(object sender, EventArgs e)
        {
            try
            {
                if (PCM == null || PCM.buf == null | PCM.buf.Length == 0)
                {
                    Logger("Nothing to save");
                    return;
                }
                string FileName = SelectSaveFile("BIN files (*.bin)|*.bin|ALL files(*.*)|*.*");
                if (FileName.Length == 0)
                    return;

                FixCheckSums();
                Logger("Saving to file: " + FileName);
                WriteBinToFile(FileName, PCM.buf);
                Logger("Done.");
            }
            catch (Exception ex)
            {
                Logger("Error: " + ex.Message);
            }

        }
        public void refreshTablelist()
        {
            bindingsource.DataSource = null;
            dataGridView1.DataSource = null;
            bindingsource.DataSource = tableDatas;
            dataGridView1.DataSource = bindingsource;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            
            comboTableCategory.DataSource = null;
            categoryBindingSource.DataSource = null;
            categoryBindingSource.DataSource = tableCategories;
            comboTableCategory.DataSource = categoryBindingSource;
            comboTableCategory.Refresh();
        }

        private void btnImportTableSeek_Click(object sender, EventArgs e)
        {
            for (int i=0; i< tableSeeks.Count; i++)
            {
                TableData tableData = new TableData();
                tableData.importSeekTable(i, PCM);
                tableDatas.Add(tableData);
            }
            refreshTablelist();
        }

        private void btnImportTableSearch_Click(object sender, EventArgs e)
        {
            for (int i=0; i< tableSearchResult.Count; i++)
            {

            }
        }
        private void ConvertXdf(XDocument doc)
        {
            try
            {

                List<string> categories = new List<string>();

                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFHEADER"))
                {
                    foreach (XElement cat in element.Elements("CATEGORY"))
                    {
                        string category = cat.Attribute("name").Value;
                        categories.Add(category);
                        if (!tableCategories.Contains(category))
                            tableCategories.Add(category);
                    }
                }
                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFTABLE"))
                {
                    TableData xdf = new TableData();
                    xdf.OS = PCM.OS;

                    string RowHeaders = "";
                    string ColHeaders = "";
                    string addr = "";
                    string size = "";
                    string math = "";
                    foreach (XElement axle in element.Elements("XDFAXIS"))
                    {
                        if (axle.Attribute("id").Value == "x")
                        {
                            xdf.Columns = Convert.ToUInt16(axle.Element("indexcount").Value);
                            foreach (XElement lbl in axle.Elements("LABEL"))
                            {
                                ColHeaders += lbl.Attribute("value").Value + ",";
                            }
                            ColHeaders = ColHeaders.Trim(',');
                            xdf.ColumnHeaders = ColHeaders;
                        }
                        if (axle.Attribute("id").Value == "y")
                        {
                            xdf.Rows = Convert.ToUInt16(axle.Element("indexcount").Value);
                            foreach (XElement lbl in axle.Elements("LABEL"))
                            {
                                RowHeaders += lbl.Attribute("value").Value + ",";
                            }
                            RowHeaders = RowHeaders.Trim(',');
                            xdf.RowHeaders = RowHeaders;
                        }
                        if (axle.Attribute("id").Value == "z")
                        {
                            addr = axle.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim();
                            xdf.AddrInt = Convert.ToUInt32(addr, 16);
                            xdf.Address = addr;
                            string tmp = axle.Element("EMBEDDEDDATA").Attribute("mmedelementsizebits").Value.Trim();
                            size = (Convert.ToInt32(tmp) / 8).ToString();
                            xdf.ElementSize = (byte)(Convert.ToInt32(tmp) / 8);
                            math = axle.Element("MATH").Attribute("equation").Value.Trim();
                            xdf.Math = math;
                            xdf.SavingMath = xdf.Math.Replace("*", "/");
                        }
                    }
                    xdf.TableName = element.Element("title").Value;
                    if (element.Element("units") != null)
                        xdf.Units = element.Element("units").Value;
                    if (element.Element("datatype") != null)
                        xdf.DataType = Convert.ToByte(element.Element("datatype").Value);
                    if (element.Element("EMBEDDEDDATA") != null && element.Element("EMBEDDEDDATA").Attribute("mmedtypeflags") != null)
                    {
                        xdf.Signed = Convert.ToBoolean(Convert.ToByte(element.Element("EMBEDDEDDATA").Attribute("mmedtypeflags").Value) & 1);
                        if ((Convert.ToByte(element.Element("EMBEDDEDDATA").Attribute("mmedtypeflags").Value) & 4) == 4)
                            xdf.RowMajor = true;
                        else
                            xdf.RowMajor = false;
                    }
                    int catid = Convert.ToInt16(element.Element("CATEGORYMEM").Attribute("category").Value);
                    xdf.Category = categories[catid];
                    if (element.Element("description") != null)
                        xdf.TableDescription = element.Element("description").Value;

                    tableDatas.Add(xdf);
                }
                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFCONSTANT"))
                {
                    TableData xdf = new TableData();
                    xdf.OS = PCM.OS;
                    if (element.Element("EMBEDDEDDATA").Attribute("mmedaddress") != null)
                    {
                        xdf.TableName = element.Element("title").Value;
                        xdf.AddrInt = Convert.ToUInt32(element.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim(), 16);
                        xdf.Address = element.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim();
                        xdf.ElementSize = (byte)(Convert.ToInt32(element.Element("EMBEDDEDDATA").Attribute("mmedelementsizebits").Value.Trim()) / 8);
                        xdf.Math = element.Element("MATH").Attribute("equation").Value.Trim();
                        if (element.Element("units") != null)
                            xdf.Units = element.Element("units").Value;
                        xdf.DataType = Convert.ToByte(element.Element("datatype").Value);
                        if (element.Element("EMBEDDEDDATA").Attribute("mmedtypeflags") != null)
                            xdf.Signed = Convert.ToBoolean(Convert.ToByte(element.Element("EMBEDDEDDATA").Attribute("mmedtypeflags").Value, 16) & 1);
                        xdf.Columns = 1;
                        xdf.Rows = 1;
                        xdf.RowMajor = false;
                        int catid = Convert.ToInt16(element.Element("CATEGORYMEM").Attribute("category").Value);
                        xdf.Category = categories[catid];
                        if (element.Element("description") != null)
                            xdf.Category = element.Element("description").Value;

                        tableDatas.Add(xdf);

                    }
                }
                foreach (XElement element in doc.Elements("XDFFORMAT").Elements("XDFFLAG"))
                {
                    TableData xdf = new TableData();
                    xdf.OS = PCM.OS;
                    if (element.Element("EMBEDDEDDATA").Attribute("mmedaddress") != null)
                    {
                        xdf.TableName = element.Element("title").Value;
                        xdf.AddrInt = Convert.ToUInt32(element.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim(), 16);
                        xdf.Address = element.Element("EMBEDDEDDATA").Attribute("mmedaddress").Value.Trim();
                        xdf.ElementSize = (byte)(Convert.ToInt32(element.Element("EMBEDDEDDATA").Attribute("mmedelementsizebits").Value.Trim()) / 8);
                        xdf.Math = "X";
                        xdf.Units = "Mask: " + element.Element("mask").Value;
                        xdf.Columns = 1;
                        xdf.Rows = 1;
                        xdf.RowMajor = false;
                        int catid = Convert.ToInt16(element.Element("CATEGORYMEM").Attribute("category").Value);
                        xdf.Category = categories[catid];
                        if (element.Element("description") != null)
                            xdf.Category = element.Element("description").Value;

                        tableDatas.Add(xdf);

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
                LoggerBold("XdfImport, line " + line + ": " + ex.Message);
            }

        }

        private void btnImportXdf_Click(object sender, EventArgs e)
        {
            XDocument doc;
            string fname = SelectFile("Select XDF file", "xdf files (*.xdf)|*.xdf|ALL files (*.*| *.*");
            if (fname.Length == 0)
                return;
            Logger("Importing file " + fname + "...", false);
            doc = new XDocument(new XComment(" Written " + DateTime.Now.ToString("G", DateTimeFormatInfo.InvariantInfo)), XElement.Load(fname));
            ConvertXdf(doc);
            refreshTablelist();
            Logger("Done");
        }

        private void btnLoadXml_Click(object sender, EventArgs e)
        {
            try
            {

                string defName = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + ".xml");
                string fName = SelectFile("Select XML File", "XML Files (*.xml)|*.xml|ALL Files (*.*)|*.*", defName);
                if (fName.Length == 0)
                    return;
                if (File.Exists(fName))
                {
                    Debug.WriteLine("Loading " + fName + "...");
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    System.IO.StreamReader file = new System.IO.StreamReader(fName);
                    tableDatas = (List<TableData>)reader.Deserialize(file);
                    file.Close();
                }
                for (int t = 0; t < tableDatas.Count; t++)
                {
                    string category = tableDatas[t].Category;
                    if (!tableCategories.Contains(category))
                        tableCategories.Add(category);
                }
                Logger("OK");
                refreshTablelist();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, line " + line + ": " + ex.Message);
            }
        }

        private void btnSaveXML_Click(object sender, EventArgs e)
        {
            try
            {
                string defName = Path.Combine(Application.StartupPath, "Tuner", PCM.OS + ".xml");
                string fName = SelectSaveFile("XML Files (*.xml)|*.xml|ALL Files (*.*)|*.*",defName);
                if (fName.Length == 0)
                    return;

                Logger("Saving file " + fName + "...", false);
                using (FileStream stream = new FileStream(fName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<TableData>));
                    writer.Serialize(stream, tableDatas);
                    stream.Close();
                }
                Logger(" [OK]");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, line " + line + ": " + ex.Message);
            }

        }

        private void btnImportDTC_Click(object sender, EventArgs e)
        {
            TableData td = new TableData();
            dtcCode dtc = dtcCodes[0];
            td.Address = dtc.CodeAddr;
            td.AddrInt = dtc.codeAddrInt;
            td.Category = "DTC";
            td.ColumnHeaders = "Code";
            td.Columns = 1;
            td.DataType = 2;
            td.Decimals = 0;
            td.ElementSize = 1;
            td.Math = "X";
            td.OS = PCM.OS;
            for (int i = 0; i < dtcCodes.Count; i++)
            {
                td.RowHeaders += dtcCodes[i].Code +",";
            }
            td.RowHeaders = td.RowHeaders.Trim(',');
            td.Rows = (ushort)dtcCodes.Count;
            td.SavingMath = "X";
            td.Signed = false;
            if (dtcCombined)
                td.TableDescription = "00 MIL and reporting off&#013;&#010;01 type A/no mil&#013;&#010;02 type B/no mil&#013;&#010;03 type C/no mil&#013;&#010;04 not reported/mil &#013;&#010;05 type A/mil &#013;&#010;06 type B/mil &#013;&#010;07 type c/mil";
            else
                td.TableDescription = "0 = 1 Trip, Emissions Related (MIL will illuminate IMMEDIATELY)&#013;&#010;1 = 2 Trips, Emissions Related (MIL will illuminate if the DTC is active for two consecutive drive cycles)&#013;&#010;2 = Non Emssions (MIL will NOT be illuminated, but the PCM will store the DTC)&#013;&#010;3 = Not Reported (the DTC test/algorithm is NOT functional, i.e. the DTC is Disabled)";
            td.TableName = "DTC";

            tableDatas.Add(td);

            if (!dtcCombined)
            {
                td = new TableData();
                td.TableName = "DTC MIL";
                td.Address = dtc.MilAddr;
                td.AddrInt = dtc.milAddrInt;
                td.Category = "DTC";
                td.ColumnHeaders = "MIL";
                td.Columns = 1;
                td.DataType = 2;
                td.Decimals = 0;
                td.ElementSize = 1;
                td.Math = "X";
                td.OS = PCM.OS;
                for (int i = 0; i < dtcCodes.Count; i++)
                {
                    td.RowHeaders += dtcCodes[i].Code + ",";
                }
                td.Rows = (ushort)dtcCodes.Count;
                td.SavingMath = "X";
                td.Signed = false;
                td.TableDescription = "0 = No MIL (Lamp always off)&#013;&#010;1 = MIL (Lamp may be commanded on by PCM)";
                tableDatas.Add(td);
            }
            refreshTablelist();
        }

        private void frmTuner_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                if (Properties.Settings.Default.TunerWindowSize.Width > 0 || Properties.Settings.Default.TunerWindowSize.Height > 0)
                {
                    this.WindowState = Properties.Settings.Default.TunerWindowState;
                    if (this.WindowState == FormWindowState.Minimized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    this.Location = Properties.Settings.Default.TunerWindowLocation;
                    this.Size = Properties.Settings.Default.TunerWindowSize;
                }
            }

        }
        private void frmTuner_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Properties.Settings.Default.MainWindowPersistence)
            {
                Properties.Settings.Default.TunerWindowState = this.WindowState;
                if (this.WindowState == FormWindowState.Normal)
                {
                    Properties.Settings.Default.TunerWindowLocation = this.Location;
                    Properties.Settings.Default.TunerWindowSize = this.Size;
                }
                else
                {
                    Properties.Settings.Default.TunerWindowLocation = this.RestoreBounds.Location;
                    Properties.Settings.Default.TunerWindowSize = this.RestoreBounds.Size;
                }
                Properties.Settings.Default.Save();
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            tableDatas = new List<TableData>();
            refreshTablelist();
        }

        private void comboTableCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboTableCategory.Text == "All")
                    bindingsource.DataSource = tableDatas;
                else
                {
                    filteredCategories = new BindingList<TableData>(tableDatas.Where(t => t.Category.ToLower().Contains(comboTableCategory.Text.ToLower())).ToList());
                    bindingsource.DataSource = filteredCategories;
                }
            }
            catch { }

        }

        private void btnSearchTableSeek_Click(object sender, EventArgs e)
        {
            int rowindex = dataGridView1.CurrentCell.RowIndex;
            for (int i = rowindex + 1; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells["Name"].Value.ToString().ToLower().Contains(txtSearchTableSeek.Text.ToLower()))
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = dataGridView1.Rows[i].Cells[0];
                    dataGridView1.CurrentCell.Selected = true;
                    //dataGridTableSeek.Rows[i].Cells[0].Selected = true;                    
                    break;
                }
            }

        }

        private void btnReadTinyTunerDB_Click(object sender, EventArgs e)
        {
            TinyTuner tt = new TinyTuner();
            Logger("Reading TinyTuner DB...", false);
            Logger(tt.readTinyDBtoTableData(PCM));
            refreshTablelist();

        }
    }
}
