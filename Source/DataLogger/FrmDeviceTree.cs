using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static UniversalPatcher.DataLogger;
using static Upatcher;
using static LoggerUtils;

namespace UniversalPatcher
{
    public partial class FrmDeviceTree : Form
    {
        public FrmDeviceTree( CANDevice CurrentDev, bool DtcTab)
        {
            InitializeComponent();
            //this.candevices = CanDevices.OrderBy(X=>X.ModuleID).ToList();
            this.currentdev = CurrentDev;
            SelectedDevice = currentdev;
            this.dtctab = DtcTab;
        }

        private List<CANDevice> candevices;
        private CANDevice currentdev;
        public CANDevice SelectedDevice;
        private bool dtctab;
        private void FrmDeviceTree_Load(object sender, EventArgs e)
        {
            LoadDevices();
        }

        private void LoadDevices()
        {
            treeView1.Nodes.Clear();
            treeView1.AfterSelect -= treeView1_AfterSelect;
            treeView1.NodeMouseDoubleClick -= TreeView1_NodeMouseDoubleClick;
            candevices = frmlogger.CanDevsMain; //.OrderBy(X => X.ModuleID).ToList();
            AddSubnet(LoggingProtocol.HSCAN);
            AddSubnet(LoggingProtocol.LSCAN);
            AddSubnet(LoggingProtocol.VPW);
            treeView1.AfterSelect += treeView1_AfterSelect;
            treeView1.NodeMouseDoubleClick += TreeView1_NodeMouseDoubleClick;
        }
        private void AddSubnet(LoggingProtocol subnet)
        {
            List<CANDevice> tmpDevs = candevices.Where(X => X.Subnet == subnet).ToList();
            if (tmpDevs != null && tmpDevs.Count > 0)
            {
                TreeNode tnSub = treeView1.Nodes.Add(subnet.ToString());
                tnSub.Name = subnet.ToString();
                tnSub.ImageKey = "canbus.ico";
                tnSub.SelectedImageKey = "canbus.ico";
                TreeNode tn0 = tnSub.Nodes.Add(tmpDevs[0].ModuleName + " [" + tmpDevs[0].RequestID.ToString("X4") + "]");
                tn0.Tag = tmpDevs[0];
                tn0.ImageKey = "UniversalPatcher.ico";
                tn0.SelectedImageKey = "UniversalPatcher.ico";
                if (tmpDevs[0].RequestID == currentdev.RequestID && tmpDevs[0].Subnet == currentdev.Subnet)
                {
                    treeView1.SelectedNode = tn0;
                    ShowModuleInfo(tmpDevs[0]);
                }
                List<DTCCodeStatus> codes = tmpDevs[0].DTCCodes.Where(X => X.Code != "P0000").ToList();
                if (codes != null && codes.Count > 0)
                {
                    tn0.BackColor = Color.LightPink;
                    tn0.Text += " (" + codes.Count.ToString() + " DTC)";
                }
                for (int i = 1; i < tmpDevs.Count; i++)
                {
                    TreeNode tn1 = tn0.Nodes.Add(tmpDevs[i].ModuleName + " [" + tmpDevs[i].RequestID.ToString("X4") + "]");
                    tn1.Tag = tmpDevs[i];
                    tn1.ImageKey = "Modify.ico";
                    tn1.SelectedImageKey = "Modify.ico";
                    if (tmpDevs[i].RequestID == currentdev.RequestID && tmpDevs[i].Subnet == currentdev.Subnet)
                    {
                        treeView1.SelectedNode = tn1;
                        ShowModuleInfo(tmpDevs[i]);
                    }
                    codes = tmpDevs[i].DTCCodes.Where(X => X.Code != "P0000").ToList();
                    if (codes != null && codes.Count > 0)
                    {
                        tn1.BackColor = Color.LightPink;
                        tn1.Text += " (" + codes.Count.ToString() + " DTC)";
                    }
                }
                tnSub.ExpandAll();
            }

        }

        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (treeView1.SelectedNode.Tag == null)
            {
                return;
            }
            SelectedDevice = (CANDevice)treeView1.SelectedNode.Tag;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ShowModuleInfo(CANDevice canDev)
        {
            if (canDev == null)
            {
                return;
            }
            txtModuleInfo.Text = "Module Name: " + canDev.ModuleName + Environment.NewLine;
            txtModuleInfo.AppendText( "Module Description: " + canDev.ModuleDescription + Environment.NewLine);
            txtModuleInfo.AppendText("Module ID: " + canDev.ModuleID.ToString("X") + Environment.NewLine);
            txtModuleInfo.AppendText("Res ID: " + canDev.ResID.ToString("X") + Environment.NewLine);
            txtModuleInfo.AppendText("Request ID: " + canDev.RequestID.ToString("X") + Environment.NewLine);
            txtModuleInfo.AppendText("Diag ID: " + canDev.DiagID.ToString("X") + Environment.NewLine);
            string vin = frmlogger.RequestVINCode(canDev);
            if (!string.IsNullOrEmpty(vin))
            {
                txtModuleInfo.AppendText("VIN: " + vin + Environment.NewLine);
            }
            if (canDev.DTCCodes.Count > 0)
            {
                txtModuleInfo.AppendText(Environment.NewLine +  "DTC codes: " + Environment.NewLine);
                for (int i=0;i<canDev.DTCCodes.Count;i++)
                {
                    txtModuleInfo.AppendText("---------" + Environment.NewLine +
                        "Code: " + canDev.DTCCodes[i].Code + Environment.NewLine +
                        "Description: " + canDev.DTCCodes[i].Description + Environment.NewLine +
                        "Status: " + canDev.DTCCodes[i].Status + Environment.NewLine);
                }
            }
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                SelectedDevice = (CANDevice)treeView1.SelectedNode.Tag;
                ShowModuleInfo(SelectedDevice);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                this.DialogResult = DialogResult.OK;
                frmlogger.SelectCanDevice(SelectedDevice, dtctab);
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnDTC_Click(object sender, EventArgs e)
        {
            frmlogger.getDtcCodes(false, chkDTCAll.Checked);
            LoadDevices();
        }

        private void btnQueryModules_Click(object sender, EventArgs e)
        {
            frmlogger.QueryCanDevicesMain();
            LoadDevices();
        }
    }
}
