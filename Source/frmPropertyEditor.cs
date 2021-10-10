using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmPropertyEditor : Form
    {
        public frmPropertyEditor()
        {
            InitializeComponent();
        }

        private object myObj;
        private void frmClassEditor_Load(object sender, EventArgs e)
        {

        }

        public void loadObject(object myObj)
        {
            int row = 0;
            this.myObj = myObj;
            dataGridView1.ColumnCount = 1;
            dataGridView1.RowHeadersWidth = 150;
            foreach (var prop in myObj.GetType().GetProperties())
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[row].HeaderCell.Value = prop.Name;
                if (prop.PropertyType.IsEnum)
                {
                    DataGridViewComboBoxCell c = new DataGridViewComboBoxCell();
                    c.ValueType = prop.PropertyType;
                    c.ValueMember = "Value";
                    c.DisplayMember = "Name";
                    c.DataSource = Enum.GetValues(prop.PropertyType).Cast<object>().Select(v => new
                    {
                        Value = (int)v,
                        Name = Enum.GetName(prop.PropertyType, v) /* or any other logic to get text */
                    }).ToList();
                    c.Value = (int)prop.GetValue(myObj, null);
                    dataGridView1.Rows[row].Cells[0] = c;
                }
                else if (prop.PropertyType == typeof(System.Boolean))
                {
                    DataGridViewCheckBoxCell c = new DataGridViewCheckBoxCell();
                    dataGridView1.Rows[row].Cells[0] = c;
                    c.Value = prop.GetValue(myObj, null);
                }
                else
                {
                    dataGridView1.Rows[row].Cells[0].Value = prop.GetValue(myObj, null);
                }
                row++;
            }
            //dataGridView1.Columns[0].Width = 1000;
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].HeaderCell.Value != null)
                {
                    string propertyName = dataGridView1.Rows[i].HeaderCell.Value.ToString();
                    var propertyInfo = myObj.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
                    if (propertyInfo != null && propertyInfo.CanWrite)
                    {
                        if (dataGridView1.Rows[i].Cells[0].GetType() == typeof(DataGridViewComboBoxCell))
                            propertyInfo.SetValue(myObj, Enum.ToObject(propertyInfo.PropertyType, dataGridView1.Rows[i].Cells[0].Value), null);
                        else
                            propertyInfo.SetValue(myObj, Convert.ChangeType(dataGridView1.Rows[i].Cells[0].Value, propertyInfo.PropertyType), null);
                    }
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CopyToClipboard()
        {
            //Copy to clipboard
            DataObject dataObj = dataGridView1.GetClipboardContent();
            if (dataObj != null)
                Clipboard.SetDataObject(dataObj);
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //Copy to clipboard
                CopyToClipboard();

                //Clear selected cells
                foreach (DataGridViewCell dgvCell in dataGridView1.SelectedCells)
                    dgvCell.Value = string.Empty;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }

        private void PasteClipboardValue()
        {
            try
            {
                //Show Error if no cell is selected
                if (dataGridView1.SelectedCells.Count == 0)
                {
                    MessageBox.Show("Please select a cell", "Paste",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //Get the starting Cell
                DataGridViewCell startCell = GetStartCell(dataGridView1);
                //Get the clipboard value in a dictionary
                Dictionary<int, Dictionary<int, string>> cbValue =
                        ClipBoardValues(Clipboard.GetText());

                int iRowIndex = startCell.RowIndex;
                foreach (int rowKey in cbValue.Keys)
                {
                    int iColIndex = startCell.ColumnIndex;
                    foreach (int cellKey in cbValue[rowKey].Keys)
                    {
                        //Check if the index is within the limit
                        if (iColIndex <= dataGridView1.Columns.Count - 1
                        && iRowIndex <= dataGridView1.Rows.Count - 1)
                        {
                            DataGridViewCell cell = dataGridView1[iColIndex, iRowIndex];

                            //Copy to selected cells if 'chkPasteToSelectedCells' is checked
                            /*if ((chkPasteToSelectedCells.Checked && cell.Selected) ||
                                (!chkPasteToSelectedCells.Checked))*/
                            cell.Value = cbValue[rowKey][cellKey];
                        }
                        iColIndex++;
                    }
                    iRowIndex++;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmTuner reorderColumns, line " + line + ": " + ex.Message);
            }

        }

        private DataGridViewCell GetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;

            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;

            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }

            return dgView[colIndex, rowIndex];
        }

        private Dictionary<int, Dictionary<int, string>> ClipBoardValues(string clipboardValue)
        {
            Dictionary<int, Dictionary<int, string>>
            copyValues = new Dictionary<int, Dictionary<int, string>>();

            String[] lines = clipboardValue.Split('\n');

            for (int i = 0; i <= lines.Length - 1; i++)
            {
                copyValues[i] = new Dictionary<int, string>();
                String[] lineContent = lines[i].Split('\t');

                //if an empty cell value copied, then set the dictionary with an empty string
                //else Set value to dictionary
                if (lineContent.Length == 0)
                    copyValues[i][0] = string.Empty;
                else
                {
                    for (int j = 0; j <= lineContent.Length - 1; j++)
                        copyValues[i][j] = lineContent[j];
                }
            }
            return copyValues;
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteClipboardValue();
        }
    }
}
