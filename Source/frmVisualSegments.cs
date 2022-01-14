using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static Upatcher;

namespace UniversalPatcher
{
    public partial class frmVisualSegments : Form
    {
        public frmVisualSegments()
        {
            InitializeComponent();
        }

        private struct SegBlock
        {
            public uint start;
            public uint end;
            public uint size;
            public string name;
        }
        private PcmFile PCM;

        public void showChart(PcmFile PCM)
        {
            chart1.Dock = DockStyle.Fill;
            uint minAddr = uint.MaxValue;
            uint maxAddr = 0;
            this.PCM = PCM;

            try
            {
                List<SegBlock> segBlocks = new List<SegBlock>();
                chart1.Series.Clear();

                for (int s = 0; s < PCM.Segments.Count; s++)    //Find first and last address of segments
                {
                    if (PCM.Segments[s].Name == "OS" && !chkShowOS.Checked)
                        continue;
                    for (int b = 0; b < PCM.segmentAddressDatas[s].SegmentBlocks.Count; b++)
                    {
                        if (PCM.segmentAddressDatas[s].SegmentBlocks[b].Start < minAddr)
                            minAddr = PCM.segmentAddressDatas[s].SegmentBlocks[b].Start;
                        if (PCM.segmentAddressDatas[s].SegmentBlocks[b].End > maxAddr)
                            maxAddr = PCM.segmentAddressDatas[s].SegmentBlocks[b].End;
                    }
                }

                if (minAddr > 0 && chkShowOS.Checked)
                {
                    SegBlock sb = new SegBlock();
                    sb.name = "Free";
                    sb.size = minAddr;
                    sb.start = 0;
                    sb.end = minAddr;
                    segBlocks.Add(sb);
                }

                for (int s = 0; s < PCM.Segments.Count; s++)
                {
                    if (PCM.Segments[s].Name == "OS" && !chkShowOS.Checked)
                        continue;
                    for (int b = 0; b < PCM.segmentAddressDatas[s].SegmentBlocks.Count; b++)
                    {
                        SegBlock sb = new SegBlock();
                        sb.name = PCM.Segments[s].Name;
                        sb.size = PCM.segmentAddressDatas[s].SegmentBlocks[b].End - PCM.segmentAddressDatas[s].SegmentBlocks[b].Start + 1;
                        sb.start = PCM.segmentAddressDatas[s].SegmentBlocks[b].Start;
                        sb.end = PCM.segmentAddressDatas[s].SegmentBlocks[b].End;
                        segBlocks.Add(sb);
                    }
                }

                if (maxAddr < PCM.fsize)
                {
                    SegBlock sb = new SegBlock();
                    sb.name = "Free";
                    sb.size = PCM.fsize - maxAddr - 1;
                    sb.start = maxAddr + 1;
                    sb.end = PCM.fsize;
                    segBlocks.Add(sb);
                }

                List<SegBlock> orderedBlocks = new List<SegBlock>();
                while (segBlocks.Count > 0)
                {
                    int smallestInd = 0;
                    for (int i = 0; i < segBlocks.Count; i++)
                    {
                        if (segBlocks[i].start < segBlocks[smallestInd].start)
                            smallestInd = i;
                    }

                    orderedBlocks.Add(segBlocks[smallestInd]);
                    segBlocks.RemoveAt(smallestInd);
                }
                //Check if there is any free space between blocks:
                for (int b = 0; b < orderedBlocks.Count - 1; b++)
                {
                    if (orderedBlocks[b].end < (orderedBlocks[b + 1].start - 1) && chkShowOS.Checked)
                    {
                        SegBlock sb = new SegBlock();
                        sb.name = "Free";
                        sb.start = orderedBlocks[b].end + 1;
                        sb.end = orderedBlocks[b + 1].start - 1;
                        sb.size = sb.end - sb.start;
                        segBlocks.Add(sb);
                    }
                    segBlocks.Add(orderedBlocks[b]);
                }
                //string[] x = new string[segBlocks.Count];
                //((uint[] y = new uint[segBlocks.Count];

                for (int b = 0; b < segBlocks.Count; b++)
                {
                    chart1.Series.Add(segBlocks[b].name + "[" + segBlocks[b].start.ToString("X") + "-" + segBlocks[b].end.ToString("X") + "]");
                    chart1.Series[b].ChartType = SeriesChartType.StackedBar;
                    chart1.Series[b].Points.Add(segBlocks[b].size);
                    //x[b] = segBlocks[b].name;
                    //y[b] = segBlocks[b].size;
                }


                //chart1.Series[0].Points.DataBindXY(x, y);
                chart1.Legends[0].Enabled = true;
                chart1.ChartAreas[0].Area3DStyle.Enable3D = false;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }
        private void frmVisualSegments_Load(object sender, EventArgs e)
        {
        }

        private void chkShowOS_CheckedChanged(object sender, EventArgs e)
        {
            showChart(PCM);
        }
    }
}
