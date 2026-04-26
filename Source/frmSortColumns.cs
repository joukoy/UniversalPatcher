using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;
using System.IO;

namespace UniversalPatcher
{
    public partial class frmSortColumns : Form
    {
        public frmSortColumns(List<ColumnOrder> cols)
        {
            InitializeComponent();
            columns = cols;
        }

        public List<ColumnOrder> columns;
        private string colsFile;

        private Rectangle dragBoxFromMouseDown;
        private int rowIndexFromMouseDown;
        private int rowIndexOfItemUnderMouseToDrop;
        private void frmSortColumns2_Load(object sender, EventArgs e)
        {
            //dataGridView1.DataSource = columns;
            colsFile = ColumnsFile;
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn();
            checkColumn.Name = "Visible";
            checkColumn.HeaderText = "Visible";
            checkColumn.Width = 50;
            checkColumn.ReadOnly = false;
            dataGridView1.Columns.Add(checkColumn);
            dataGridView1.Columns.Add("Column", "Column");
            dataGridView1.Columns.Add("Width", "Width");
            foreach (ColumnOrder co in columns)
            {
                int r = dataGridView1.Rows.Add();
                dataGridView1.Rows[r].Cells["Visible"].Value = co.Visible;
                dataGridView1.Rows[r].Cells["Column"].Value = co.Column;
                dataGridView1.Rows[r].Cells["Width"].Value = co.Width;
            }
            dataGridView1.MouseMove += DataGridView1_MouseMove;
            dataGridView1.MouseDown += DataGridView1_MouseDown;
            dataGridView1.DragDrop += DataGridView1_DragDrop;
            dataGridView1.DragOver += DataGridView1_DragOver;
        }

        private void DataGridView1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void DataGridView1_DragDrop(object sender, DragEventArgs e)
        {
            // The mouse locations are relative to the screen, so they must be 
            // converted to client coordinates.
            Point clientPoint = dataGridView1.PointToClient(new Point(e.X, e.Y));

            // Get the row index of the item the mouse is below. 
            rowIndexOfItemUnderMouseToDrop =
                dataGridView1.HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // If the drag operation was a move then remove and insert the row.
            if (e.Effect == DragDropEffects.Move)
            {
                DataGridViewRow rowToMove = e.Data.GetData(
                    typeof(DataGridViewRow)) as DataGridViewRow;
                dataGridView1.Rows.RemoveAt(rowIndexFromMouseDown);
                dataGridView1.Rows.Insert(rowIndexOfItemUnderMouseToDrop, rowToMove);

            }
        }

        private void DataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            rowIndexFromMouseDown = dataGridView1.HitTest(e.X, e.Y).RowIndex;
            if (rowIndexFromMouseDown != -1)
            {
                // Remember the point where the mouse down occurred. 
                // The DragSize indicates the size that the mouse can move 
                // before a drag event should be started.                
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                               e.Y - (dragSize.Height / 2)),
                                    dragSize);
            }
            else
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void DataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {

                    // Proceed with the drag and drop, passing in the list item.                    
                    DragDropEffects dropEffect = dataGridView1.DoDragDrop(
                    dataGridView1.Rows[rowIndexFromMouseDown],
                    DragDropEffects.Move);
                }
            }
        }

        private void ApplyChanges()
        {
            columns = new List<ColumnOrder>();
            for (int r = 0; r < dataGridView1.Rows.Count; r++)
            {
                ColumnOrder co = new ColumnOrder();
                co.Column = dataGridView1.Rows[r].Cells["Column"].Value.ToString();
                co.Visible = Convert.ToBoolean(dataGridView1.Rows[r].Cells["Visible"].Value);
                co.Width = Convert.ToInt32(dataGridView1.Rows[r].Cells["Width"].Value);
                columns.Add(co);
            }
            ColumnsFile = colsFile;

        }
        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int r=0;r<dataGridView1.Rows.Count;r++)
            {
                dataGridView1.Rows[r].Cells["Visible"].Value = chkSelectAll.Checked;
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            if (SaveColumnsPreset(columns, null))
            {
                colsFile = ColumnsFile;
            }
         }
    }
}
