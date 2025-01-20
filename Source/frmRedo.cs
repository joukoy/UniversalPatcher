using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;
using System.Reflection;

namespace UniversalPatcher
{
    public partial class frmRedo : Form
    {
        public frmRedo()
        {
            InitializeComponent();
        }

        private void frmRedo_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = RedoLog;
            uPLogger.UpLogUpdated += UPLogger_UpLogUpdated;
            this.FormClosing += FrmRedo_FormClosing;
        }

        private void FrmRedo_FormClosing(object sender, FormClosingEventArgs e)
        {
            uPLogger.UpLogUpdated -= UPLogger_UpLogUpdated;
        }

        private void UPLogger_UpLogUpdated(object sender, UPLogger.UPLogString e)
        {
            uPLogger.DisplayText(e.LogText, e.Bold, richTextBox1);
        }

        private void SetValue(ReDo redo, Object val)
        {
            try
            {
                var propertyInfo = redo.Obj.GetType().GetProperty(redo.Property, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    if (propertyInfo.PropertyType.IsEnum)
                        propertyInfo.SetValue(redo.Obj, Enum.ToObject(propertyInfo.PropertyType, val), null);
                        //propertyInfo.SetValue(redo.Obj, Enum.Parse(propertyInfo.PropertyType, val.ToString()), null);
                    else if (propertyInfo.PropertyType == typeof(SerializableFont))
                        propertyInfo.SetValue(redo.Obj, new SerializableFont(val.ToString()), null);
                    else
                        propertyInfo.SetValue(redo.Obj, Convert.ChangeType(val, propertyInfo.PropertyType), null);
                    if (val == null)
                        Logger("Collection: " + redo.Collection + ", Item: " + redo.ItemName + ", Property: " + redo.Property + ", set value: (null)");
                    else
                        Logger("Collection: " + redo.Collection + ", Item: " + redo.ItemName + ", Property: " + redo.Property + ", set value: " + val.ToString());
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, frmRedo , line " + line + ": " + ex.Message);
            }
        }

        private void AddItem(ReDo redo)
        {
            System.Collections.ICollection collection = redo.ObjCollection as System.Collections.ICollection;
            if (redo.Position > -1 && redo.Position < collection.Count)
            {
                MethodInfo mi = redo.ObjCollection.GetType().GetMethod("Insert");
                mi.Invoke(redo.ObjCollection, new object[] { redo.Position, redo.Obj });
            }
            else
            {
                MethodInfo mi = redo.ObjCollection.GetType().GetMethod("Add");
                mi.Invoke(redo.ObjCollection, new object[] { redo.Obj });
            }
            Logger("Adding " + redo.ItemName + " to " + redo.Collection);

        }

        private void RemoveItem(ReDo redo)
        {
            Logger("Removing " + redo.ItemName + " from " + redo.Collection);
            MethodInfo mi = redo.ObjCollection.GetType().GetMethod("Remove");
            mi.Invoke(redo.ObjCollection, new object[] { redo.Obj });

        }
        private void btnUndo_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    return;
                }
                for (int r = 0; r < dataGridView1.SelectedRows.Count; r++)
                {
                    ReDo redo = (ReDo)dataGridView1.SelectedRows[r].DataBoundItem;
                    if (redo.Action == ReDo.RedoAction.Delete)
                    {
                        AddItem(redo);
                    }
                    else if (redo.Action == ReDo.RedoAction.Add)
                    {
                        RemoveItem(redo);
                    }
                    else if (redo.Action == ReDo.RedoAction.Edit)
                    {
                        SetValue(redo, redo.OldValue);
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
                LoggerBold("Error, frmRedo , line " + line + ": " + ex.Message);
            }

        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    return;
                }
                for (int r = 0; r < dataGridView1.SelectedRows.Count; r++)
                {
                    ReDo redo = (ReDo)dataGridView1.SelectedRows[r].DataBoundItem;
                    if (redo.Action == ReDo.RedoAction.Delete)
                    {
                        RemoveItem(redo);
                    }
                    else if (redo.Action == ReDo.RedoAction.Add)
                    {
                        AddItem(redo);
                    }
                    else if (redo.Action == ReDo.RedoAction.Edit)
                    {
                        SetValue(redo, redo.NewValue);
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
                LoggerBold("Error, frmRedo , line " + line + ": " + ex.Message);
            }
        }

        private void FilterRedo()
        {
            List<ReDo> filtered = RedoLog.ToList();
            if (txtFilterCollections.Text.Length > 0)
            {
                filtered = RedoLog.Where(X => X.Collection.ToLower().Contains(txtFilterCollections.Text.ToLower())).ToList();
            }
            if (txtFilterItems.Text.Length > 0)
            {
                filtered = RedoLog.Where(X => X.ItemName.ToLower().Contains(txtFilterItems.Text.ToLower())).ToList();
            }
            dataGridView1.DataSource = filtered;
        }
        private void txtFilterCollections_TextChanged(object sender, EventArgs e)
        {
            FilterRedo();
        }

        private void txtFilterItems_TextChanged(object sender, EventArgs e)
        {
            FilterRedo();
        }

        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtFilterCollections.Text = "";
            txtFilterItems.Text = "";
        }
    }
}
