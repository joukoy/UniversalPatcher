using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmImportFile : Form
    {
        public frmImportFile()
        {
            InitializeComponent();
        }

        public string outFileName = "";

        private void frmImportFile_Load(object sender, EventArgs e)
        {
            LogReceivers.Add(txtResult);
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
            txtResult.AppendText(LogText);
            if (NewLine)
                txtResult.AppendText(Environment.NewLine);
            Application.DoEvents();
        }

        public void importMotorola(string fileName)
        {
            try
            {
                this.Text = "Import Motorola S-Record";
                labelFileName.Text = fileName;
                SRecord srecord = new SRecord();
                StreamReader file = new StreamReader(labelFileName.Text);
                SRecordStructure sRec = srecord.Read(file); //Skip first block. Header ?
                Logger("Starting address:" + sRec.address.ToString("X") + "(HEX), " + sRec.address.ToString() + " (DEC)");
                txtOffset.Text = sRec.address.ToString("X");
                file.Close();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        public void importIntel(string fileName)
        {
            try
            {
                this.Text = "Import Intel HEX";
                labelFileName.Text = fileName;
                IntelHex intelHex = new IntelHex();
                StreamReader file = new StreamReader(labelFileName.Text);
                //IntelHexStructure ihex = intelHex.Read(file); //Skip first block. Header ?
                while (true)
                {
                    IntelHexStructure ihex = intelHex.Read(file);
                    if (ihex == null)
                        break;
                    if (ihex.type == 4)
                    {
                        Logger("Segment address:" + ihex.address.ToString("X") + " (HEX), " + ihex.address.ToString() + " (DEC)");
                    }
                    if (ihex.type == 0) //First datablock
                    {
                        Logger("Starting address:" + ihex.address.ToString("X") + " (HEX), " + ihex.address.ToString() + " (DEC)");
                        txtOffset.Text = ihex.address.ToString("X");
                        break;
                    }

                }
                file.Close();


            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string defName = Path.GetFileNameWithoutExtension(labelFileName.Text) + ".bin";
                outFileName = SelectSaveFile("BIN (*.bin)|*.bin|All(*.*)|*.*", defName);
                if (outFileName.Length == 0)
                    return;
                Logger("Saving to file: " + outFileName, false);
                FileStream stream = new FileStream(outFileName, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(stream);
                uint offset = 0;
                if (!HexToUint(txtOffset.Text, out offset))
                {
                    LoggerBold("Can't decode HEX offset: " + txtOffset.Text);
                    return;
                }
                if (this.Text.Contains("Motorola"))
                {
                    StreamReader file = new StreamReader(labelFileName.Text);
                    SRecord srecord = new SRecord();
                    while (true)
                    {
                        SRecordStructure sRec = srecord.Read(file);
                        if (sRec == null)
                            break;
                        if (sRec.type > 0 && sRec.type < 4) //Data records are type  1 - 3
                        {
                            writer.Seek((int)(sRec.address - offset), SeekOrigin.Begin);
                            writer.Write(sRec.data, 0, sRec.dataLen);
                        }
                    }
                    file.Close();
                
                }
                else
                {
                    IntelHex intelHex = new IntelHex();
                    StreamReader file = new StreamReader(labelFileName.Text);
                    //IntelHexStructure ihex = intelHex.Read(file); //Skip first block. Header ?
                    while (true)
                    {
                        IntelHexStructure ihex = intelHex.Read(file);
                        if (ihex == null)
                            break;
                        if (ihex.type == 0) //Datablock
                        {
                            writer.Seek((int)(ihex.address - offset), SeekOrigin.Begin);
                            writer.Write(ihex.data, 0, ihex.dataLen);
                        }
                    }

                }
                writer.Close();
                Logger(" [OK]");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }


    }
}
