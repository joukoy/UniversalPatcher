using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher.Properties
{
    public partial class frmSearchText : Form
    {
        public frmSearchText()
        {
            InitializeComponent();
        }

        private RichTextBox rtb;
        private List<int> found = null;

        public void initMe(RichTextBox rt)
        {
            rtb = rt;
            found = new List<int>();
            clearHighlights(rtb);
        }
        private void btnSearchNext_Click(object sender, EventArgs e)
        {
            if (found.Count == 0)
                searchAll();
            int pos = -1;
            for (int f = 0; f < found.Count; f++)
            { 
                if (found[f] > rtb.SelectionStart) 
                { 
                    pos = found[f]; 
                    break; 
                }
            }
            if (pos >= 0)
            { 
                rtb.Select(pos, txtSearch.Text.Length);
                rtb.ScrollToCaret();
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            found = new List<int>();         // clear list
            clearHighlights(rtb);            // clear highlights
        }

        private void btnSearchAll_Click(object sender, EventArgs e)
        {
            int cursorPos = rtb.SelectionStart;
            clearHighlights(rtb);
            found = FindAll(rtb, txtSearch.Text, 0);
            HighlightAll(rtb, Color.Red, found, txtSearch.Text.Length);
            rtb.Select(cursorPos, 0);
        }

        private void searchAll()
        {
            int cursorPos = rtb.SelectionStart;
            clearHighlights(rtb);
            found = FindAll(rtb, txtSearch.Text, 0);
            HighlightAll(rtb, Color.Red, found, txtSearch.Text.Length);
            if (found.Count > 0)
                rtb.Select(found[0], txtSearch.Text.Length);
            else
                rtb.Select(cursorPos, 0);

        }
        private void btnSearchPrev_Click(object sender, EventArgs e)
        {
            if (found.Count == 0)
                searchAll();
            int pos = -1;
            for (int f = 0; f < found.Count; f++)
            { 
                if (found[f] >= rtb.SelectionStart) 
                { 
                    if (f >= 1)
                    { 
                        pos = found[f - 1]; 
                        break;
                    }
                }
            }
            if (pos >= 0)
            {
                rtb.Select(pos, txtSearch.Text.Length);
                rtb.ScrollToCaret();
            }
        }
        public List<int> FindAll(RichTextBox rtb, string txtToSearch, int searchStart)
        {
            List<int> found = new List<int>();
            if (txtToSearch.Length <= 0) return found;

            int pos = rtb.Find(txtToSearch, searchStart, RichTextBoxFinds.None);
            while (pos >= 0)
            {
                found.Add(pos);
                pos = rtb.Find(txtToSearch, pos + txtToSearch.Length, RichTextBoxFinds.None);
            }
            return found;
        }
        public void HighlightAll(RichTextBox rtb, Color color, List<int> found, int length)
        {
            foreach (int p in found)
            {
                rtb.Select(p, length);
                rtb.SelectionColor = color;
            }
        }
        void clearHighlights(RichTextBox rtb)
        {
            int cursorPos = rtb.SelectionStart;    // store cursor
            rtb.Select(0, rtb.TextLength);         // select all
            rtb.SelectionColor = rtb.ForeColor;    // default text color
            rtb.Select(cursorPos, 0);              // reset cursor
        }

        private void frmSearchText_UnLoad(object sender, FormClosingEventArgs e)
        {
            clearHighlights(rtb);
        }
    }
}
