using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using static Upatcher;

namespace UniversalPatcher
{
    public static class ExtensionMethods
    {
        public static void EnableContextMenu(this RichTextBox rtb)
        {
            if (rtb.ContextMenuStrip == null)
            {
                // Create a ContextMenuStrip without icons
                ContextMenuStrip cms = new ContextMenuStrip();
                cms.ShowImageMargin = false;

                // 1. Add the Undo option
                ToolStripMenuItem tsmiUndo = new ToolStripMenuItem("Undo");
                tsmiUndo.Click += (sender, e) => rtb.Undo();
                cms.Items.Add(tsmiUndo);

                // 2. Add the Redo option
                ToolStripMenuItem tsmiRedo = new ToolStripMenuItem("Redo");
                tsmiRedo.Click += (sender, e) => rtb.Redo();
                cms.Items.Add(tsmiRedo);

                // Add a Separator
                cms.Items.Add(new ToolStripSeparator());

                // 3. Add the Cut option (cuts the selected text inside the richtextbox)
                ToolStripMenuItem tsmiCut = new ToolStripMenuItem("Cut");
                tsmiCut.Click += (sender, e) => rtb.Cut();
                cms.Items.Add(tsmiCut);

                // 4. Add the Copy option (copies the selected text inside the richtextbox)
                ToolStripMenuItem tsmiCopy = new ToolStripMenuItem("Copy");
                tsmiCopy.Click += (sender, e) => rtb.Copy();
                cms.Items.Add(tsmiCopy);

                // 5. Add the Paste option (adds the text from the clipboard into the richtextbox)
                ToolStripMenuItem tsmiPaste = new ToolStripMenuItem("Paste");
                tsmiPaste.Click += (sender, e) => rtb.Paste();
                cms.Items.Add(tsmiPaste);

                // 6. Add the Delete Option (remove the selected text in the richtextbox)
                ToolStripMenuItem tsmiDelete = new ToolStripMenuItem("Delete");
                tsmiDelete.Click += (sender, e) => rtb.SelectedText = "";
                cms.Items.Add(tsmiDelete);

                // Add a Separator
                cms.Items.Add(new ToolStripSeparator());

                // 7. Add the Select All Option (selects all the text inside the richtextbox)
                ToolStripMenuItem tsmiSelectAll = new ToolStripMenuItem("Select All");
                tsmiSelectAll.Click += (sender, e) => rtb.SelectAll();
                cms.Items.Add(tsmiSelectAll);

                //8. Add Font selection
                ToolStripMenuItem tsmiFont = new ToolStripMenuItem("Font");
                tsmiFont.Click += (sender, e) => TsmiFont_Click(sender, e, rtb);
                cms.Items.Add(tsmiFont);

                ToolStripMenuItem tsmiSavefile = new ToolStripMenuItem("Save...");
                tsmiSavefile.Click += (sender, e) => TsmiSavefile_Click(sender, e, rtb);
                cms.Items.Add(tsmiSavefile);

                ToolStripMenuItem tsmiSearch = new ToolStripMenuItem("Search...");
                tsmiSearch.Click += (sender, e) => TsmiSearch_Click(sender, e, rtb);
                cms.Items.Add(tsmiSearch);

                // When opening the menu, check if the condition is fulfilled 
                // in order to enable the action
                cms.Opening += (sender, e) =>
                {
                    tsmiUndo.Enabled = !rtb.ReadOnly && rtb.CanUndo;
                    tsmiRedo.Enabled = !rtb.ReadOnly && rtb.CanRedo;
                    tsmiCut.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
                    tsmiCopy.Enabled = rtb.SelectionLength > 0;
                    tsmiPaste.Enabled = !rtb.ReadOnly && Clipboard.ContainsText();
                    tsmiDelete.Enabled = !rtb.ReadOnly && rtb.SelectionLength > 0;
                    tsmiSelectAll.Enabled = rtb.TextLength > 0 && rtb.SelectionLength < rtb.TextLength;
                };

                rtb.ContextMenuStrip = cms;
            }
        }

        private static void TsmiSearch_Click(object sender, EventArgs e, RichTextBox rtb)
        {
            frmSearchText frmS = new frmSearchText();
            frmS.Show();
            frmS.InitMe(rtb);
        }

        private static void TsmiSavefile_Click(object sender, EventArgs e, RichTextBox rtb)
        {
            string fName = Helpers.SelectSaveFile(Helpers.RtfFilter);
            if (fName.Length == 0)
                return;
            rtb.SaveFile(fName);
        }

        private static void TsmiFont_Click(object sender, EventArgs e,RichTextBox rtb)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowColor = true;
            fontDlg.ShowApply = true;
            fontDlg.ShowEffects = true;
            fontDlg.ShowHelp = true;
            fontDlg.Font = rtb.Font;
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
                rtb.Font = fontDlg.Font;
                if (rtb.Name == "txtResult")
                    AppSettings.PatcherLogFont = SerializableFont.FromFont(fontDlg.Font);
                else
                    AppSettings.DebugFont = SerializableFont.FromFont(fontDlg.Font);
                AppSettings.Save();
            }
            fontDlg.Dispose();
        }

        public static class DrawingControl
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        /// <summary>
        /// Some controls, such as the DataGridView, do not allow setting the DoubleBuffered property.
        /// It is set as a protected property. This method is a work-around to allow setting it.
        /// Call this in the constructor just after InitializeComponent().
        /// </summary>
        /// <param name="control">The Control on which to set DoubleBuffered to true.</param>
        public static void SetDoubleBuffered(Control control)
        {
            // if not remote desktop session then enable double-buffering optimization
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {

                // set instance non-public property with name "DoubleBuffered" to true
                typeof(Control).InvokeMember("DoubleBuffered",
                                             System.Reflection.BindingFlags.SetProperty |
                                                System.Reflection.BindingFlags.Instance |
                                                System.Reflection.BindingFlags.NonPublic,
                                             null,
                                             control,
                                             new object[] { true });
            }
        }

        /// <summary>
        /// Suspend drawing updates for the specified control. After the control has been updated
        /// call DrawingControl.ResumeDrawing(Control control).
        /// </summary>
        /// <param name="control">The control to suspend draw updates on.</param>
        public static void SuspendDrawing(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, false, 0);
        }

        /// <summary>
        /// Resume drawing updates for the specified control.
        /// </summary>
        /// <param name="control">The control to resume draw updates on.</param>
        public static void ResumeDrawing(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, true, 0);
            control.Refresh();
        }
    }
}
}
