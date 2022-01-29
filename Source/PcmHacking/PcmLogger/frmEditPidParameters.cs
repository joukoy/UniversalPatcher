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
using System.Xml.Linq;

namespace PcmHacking
{
    public partial class frmEditPidParameters : Form
    {
        public frmEditPidParameters()
        {
            InitializeComponent();
        }
        private XDocument editXdoc;
        private int parameterEditIndex;
        private string editXdocFileName;
        private const string parameterFilter = "Parameters (*.xml)|*.xml|All Files|*.*";

        private void OpenParameterFile(string FileName)
        {
            try
            {
                parameterEditGrid.Rows.Clear();
                labelParameterEditFIle.Text = Path.GetFileName(FileName);
                editXdocFileName = FileName;
                editXdoc = XDocument.Load(FileName);
                int index = 0;
                foreach (XElement parameterElement in editXdoc.Root.Elements())
                {
                    int r = parameterEditGrid.Rows.Add();
                    parameterEditGrid.Rows[r].Cells["paramId"].Value = parameterElement.Attribute("id").Value;
                    parameterEditGrid.Rows[r].Cells["paramName"].Value = (string)parameterElement.Attribute("name").Value;

                    DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)parameterEditGrid.Rows[r].Cells["paramUnits"];
                    //cell.DisplayMember = "Units";
                    //cell.ValueMember = "Units";
                    foreach (XElement conversion in parameterElement.Elements("Conversion"))
                    {
                        cell.Items.Add(conversion.Attribute("units").Value);
                    }
                    parameterEditGrid.Rows[r].Cells["paramUnits"].Value = parameterElement.Elements("Conversion").First();
                    parameterEditGrid.Rows[r].Cells["paramIndex"].Value = index;
                    index++;
                }
                //parameterEditGrid.CellContentClick += ParameterEditGrid_CellContentClick;
                parameterEditGrid.SelectionChanged += ParameterEditGrid_SelectionChanged;
                parameterEditGrid.DataError += ParameterEditGrid_DataError;
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("OpenParameterFile, line " + line + ": " + ex.Message + Environment.NewLine);
            }
        }

        private void ParameterEditGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

        private void ParameterEditGrid_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int currentRow = parameterEditGrid.CurrentCell.RowIndex;
                valueEditGrid.Rows.Clear();
                //int r = valueEditGrid.Rows.Add();
                parameterEditIndex = 0;
                XElement parameterElement = editXdoc.Root.Elements().FirstOrDefault();
                foreach (XElement pElement in editXdoc.Root.Elements())
                {
                    if (parameterEditIndex == Convert.ToInt32(parameterEditGrid.Rows[currentRow].Cells["paramIndex"].Value))
                    {
                        parameterElement = pElement;
                        break;
                    }
                    parameterEditIndex++;
                }
                foreach (XAttribute xa in parameterElement.Attributes())
                {
                    int vr = valueEditGrid.Rows.Add();
                    valueEditGrid.Rows[vr].Cells["valParameter"].Value = xa.Name;
                    valueEditGrid.Rows[vr].Cells["valValue"].Value = xa.Value;
                }

                conversionEditGrid.Columns.Clear();
                XElement conv = parameterElement.Elements("Conversion").First();
                foreach (XAttribute xa in conv.Attributes())
                {
                    conversionEditGrid.Columns.Add(xa.Name.ToString(), xa.Name.ToString());
                }
                foreach (XElement conversion in parameterElement.Elements("Conversion"))
                {
                    int cr = conversionEditGrid.Rows.Add();
                    foreach (XAttribute xa in conversion.Attributes())
                    {
                        conversionEditGrid.Rows[cr].Cells[xa.Name.ToString()].Value = xa.Value;
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
                Debug.WriteLine("Selectionchanged, line " + line + ": " + ex.Message + Environment.NewLine);
            }
        }

        private void SelectParameterEditButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = parameterFilter;
            dialog.Multiselect = false;
            dialog.Title = "Open Parameter file";
            dialog.ValidateNames = true;

            DialogResult result = dialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                this.OpenParameterFile(dialog.FileName);
            }
        }

        private void ApplyParameterEditButton_Click(object sender, EventArgs e)
        {
            try
            {
                int index = 0;
                XElement parameterElement = editXdoc.Root.Elements("Parameter").FirstOrDefault();
                foreach (XElement pElement in editXdoc.Root.Elements("Parameter"))
                {
                    if (index == parameterEditIndex)
                    {
                        parameterElement = pElement;
                        break;
                    }
                    index++;
                }
                foreach (XAttribute xa in parameterElement.Attributes())
                {
                    for (int vr = 0; vr < valueEditGrid.Rows.Count - 1; vr++)
                    {
                        if (valueEditGrid.Rows[vr].Cells["valParameter"].Value.ToString() == xa.Name)
                        {
                            xa.Value = valueEditGrid.Rows[vr].Cells["valValue"].Value.ToString();
                            Debug.WriteLine("Param: " + xa.Name + ", value: " + xa.Value);
                        }
                    }
                }
                for (int cr = 0; cr < conversionEditGrid.Rows.Count - 1; cr++)
                {
                    bool found = false;
                    foreach (XElement conversion in parameterElement.Elements("Conversion"))
                    {
                        if (conversionEditGrid.Rows[cr].Cells["units"].Value != null && conversionEditGrid.Rows[cr].Cells["units"].Value.ToString() == conversion.Attribute("units").Value)
                        {
                            found = true;
                            foreach (XAttribute xa in conversion.Attributes())
                            {
                                xa.Value = conversionEditGrid.Rows[cr].Cells[xa.Name.ToString()].Value.ToString();
                                Debug.WriteLine("Conversion: " + conversion.Name + ", attribute: " + xa.Name + ", value: " + xa.Value);
                            }
                        }
                        if (!found)
                        {
                            parameterElement.Add(new XElement("Conversion"));
                            XElement cnv = parameterElement.Elements("Conversion").Last();
                            for (int c = 0; c < conversionEditGrid.Columns.Count; c++)
                            {
                                if (!string.IsNullOrEmpty(conversionEditGrid.Columns[c].HeaderText) &&
                                    conversionEditGrid.Rows[cr].Cells[c].Value != null)
                                {
                                    cnv.Add(new XAttribute(conversionEditGrid.Columns[c].HeaderText, 
                                        conversionEditGrid.Rows[cr].Cells[c].Value.ToString()));
                                }
                            }
                        }
                    }
                }

                editXdoc.Save(editXdocFileName);

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                Debug.WriteLine("ApplyParmeterEditButton, line " + line + ": " + ex.Message + Environment.NewLine);
            }
        }

    }
}
