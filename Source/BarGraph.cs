using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UniversalPatcher
{
    public class BarGraph : Panel
    {
        private Panel panel1;
        private Panel panel2;
        private TableLayoutPanel table;
        private float percentValue;

        public BarGraph()
        {
            table = new TableLayoutPanel();
            table.ColumnCount = 1;
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            table.Name = "tableLayoutPanel1";
            table.RowCount = 2;
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            table.Dock = DockStyle.Fill;
            table.Margin = new Padding(0);
            table.Padding = new Padding(0);
            table.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

            panel1 = new Panel();
            panel2 = new Panel();

            panel1.AutoSize = false;
            panel1.Dock = DockStyle.Fill;
            panel1.Padding = new Padding(0);
            panel1.Margin = new Padding(0);
            panel2.AutoSize = false;
            panel2.Dock = DockStyle.Fill;
            panel2.Padding = new Padding(0);
            panel2.Margin = new Padding(0);

            panel1.BackColor = System.Drawing.Color.White;
            panel2.BackColor = System.Drawing.Color.Green;

            table.Controls.Add(panel1, 0, 0);
            table.Controls.Add(panel2, 0, 1);

            this.Controls.Add(table);
            this.Size = new System.Drawing.Size(150, 500);

            SetValue(50);
        }
        public float GetValue()
        {
            return this.percentValue;
        }
        public void IncreaseValue()
        {
            IncrementValue(percentValue - 1);
        }
        public void DecreaseValue()
        {
            IncrementValue(percentValue + 1);
        }
        private void IncrementValue(float value)
        {
            SetValue(value);
        }
        public void SetValue(float val)
        {
            percentValue = val;
            if ((percentValue >= 0) && (percentValue <= 100))
            {
                table.RowStyles.RemoveAt(1);
                table.RowStyles.RemoveAt(0);
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 100 - percentValue));
                table.RowStyles.Add(new RowStyle(SizeType.Percent, percentValue));
            }
        }
    }
}
