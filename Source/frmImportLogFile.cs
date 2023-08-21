using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Helpers;
using static Upatcher;
using static UniversalPatcher.ExtensionMethods;
using static LoggerUtils;

namespace UniversalPatcher
{
    public partial class frmImportLogFile : Form
    {
        public frmImportLogFile()
        {
            InitializeComponent();
        }
        private void frmImportLogFile_Load(object sender, EventArgs e)
        {
            dataGridTimeStamps.DataError += DataGridTimeStamps_DataError;
            chkElapsed.Checked = AppSettings.LogImportTimeStampElapsed;
            txtFileName.Text = SelectFile("Select CSV File", CsvFilter);
        }

        private void DataGridTimeStamps_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private class TFormat
        {
            public string Value { get; set; }
            public TimestampFormat.TimeStampPart TimePart { get; set; }
        }
        BindingSource bindingsource = new BindingSource();
        List<TFormat> tformats = new List<TFormat>();
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            string fName = SelectFile("Select CSV File", CsvFilter);
            if (fName.Length == 0)
            {
                return;
            }
            txtFileName.Text = fName;
        }

        private void LoadTimeStamps()
        {
            try
            {
                StreamReader sr = new StreamReader(txtFileName.Text);
                string logLine = sr.ReadLine();
                logLine = sr.ReadLine();
                tformats = new List<TFormat>();
                string[] fParts = null;
                if (!string.IsNullOrEmpty(AppSettings.LogImportTimeStampFormat))
                {
                    fParts = AppSettings.LogImportTimeStampFormat.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
                string[] lParts = logLine.Split(new string[] { AppSettings.LoggerLogSeparator }, StringSplitOptions.None);
                string[] tParts = lParts[0].Split(new char[] { ' ', '.', ':', '-', ';' }, StringSplitOptions.RemoveEmptyEntries);
                labelTstampString.Text = "Timestamp: " + lParts[0];
                bool useOldFormat = false;
                for (int t=0;t<tParts.Length;t++)
                {
                    //int row= dataGridTimeStamps.Rows.Add();
                    //dataGridTimeStamps.Rows[row].Cells[0].Value = tParts[t];
                    TFormat tf = new TFormat();
                    tf.Value = tParts[t];
                    if (fParts != null && fParts.Length == tParts.Length)
                    {
                        //dataGridTimeStamps.Rows[row].Cells[1].Value = fParts[t];
                        tf.TimePart = (TimestampFormat.TimeStampPart)Enum.Parse(typeof(TimestampFormat.TimeStampPart), fParts[t]);
                        useOldFormat = true;
                    }
                    tformats.Add(tf);
                }
                if (!useOldFormat)
                {
                    if (tformats.Count <3)
                    {
                        chkElapsed.Checked = true;
                    }
                    else
                    {
                        chkElapsed.Checked = false;
                    }
                    int f = tformats.Count - 1;
                    if (f > 0)
                    {
                        tformats[f].TimePart = TimestampFormat.TimeStampPart.SecondDecimal;
                    }
                    else
                    {
                        tformats[f].TimePart = TimestampFormat.TimeStampPart.Millisecond;
                    }
                    f--;
                    if (f >= 0)
                    {
                        tformats[f].TimePart = TimestampFormat.TimeStampPart.Second;
                    }
                    f--;
                    if (f >= 0)
                    {
                        tformats[f].TimePart = TimestampFormat.TimeStampPart.Minute;
                    }
                    f--;
                    if (f >= 0)
                    {
                        if (tParts[f].ToLower().Contains("a") || tParts[f].ToLower().Contains("p"))
                        {
                            tformats[f].TimePart = TimestampFormat.TimeStampPart.AmPm;
                            f--;
                        }
                        tformats[f].TimePart = TimestampFormat.TimeStampPart.Hour;
                    }
                    f--;
                    if (f >= 0)
                    {
                        tformats[f].TimePart = TimestampFormat.TimeStampPart.Day;
                    }
                    f--;
                    if (f >= 0)
                    {
                        tformats[f].TimePart = TimestampFormat.TimeStampPart.Month;
                    }
                    f--;
                    if (f >= 0)
                    {
                        tformats[f].TimePart = TimestampFormat.TimeStampPart.Year;
                    }
                }
                bindingsource.DataSource = tformats;
                dataGridTimeStamps.DataSource = bindingsource;
                UseComboBoxForEnums(dataGridTimeStamps);
                dataGridTimeStamps.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine ("Error, frmImportLogFile line " + line + ": " + ex.Message + ", inner exception: " + ex.InnerException);
            }

        }
        private void txtFileName_TextChanged(object sender, EventArgs e)
        {
            LoadTimeStamps();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            AppSettings.LogImportTimeStampFormat = "";
            for (int r=0;r<tformats.Count;r++)
            {
                AppSettings.LogImportTimeStampFormat += tformats[r].TimePart.ToString() + ",";
            }
            AppSettings.LogImportTimeStampFormat.Trim(',');
            AppSettings.LogImportTimeStampElapsed = chkElapsed.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

    }
}
