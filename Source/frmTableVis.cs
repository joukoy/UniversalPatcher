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
    public partial class frmTableVis : Form
    {
        public frmTableVis(PcmFile PCM,TableData td)
        {
            InitializeComponent();
            this.PCM = PCM;
            this.td = td;
            this.Text = "Table data visualizer [" + td.TableName +"]";
        }

        private PcmFile PCM;
        private TableData td;
        private uint selectedByte;
        private byte[] tableBuf;
        private int selectedTxt = 0;
        private int bufTableStart = 0;
        private int tableTxtSize = 0;        

        private class HexData
        {
            public HexData()
            {
                hex = "";
                prefix = "";
                suffix = " ";
            }
            public string hex { get; set; }
            public string prefix { get; set; }
            public string suffix { get; set; }

        }
        private List<HexData> hexDatas;

        private void frmTableVis_Load(object sender, EventArgs e)
        {
        }

/*        public static string ToHexString(byte data)
        {
            int i, j, k;
            char[] r = new char[2];
            k = data >> 4;
            r[0] = (char)(k > 9 ? k + 0x37 : k + 0x30);
            k = data & 15;
            r[1] = (char)(k > 9 ? k + 0x37 : k + 0x30);
            return new string(r);
        }
*/
        public void changeSelection(uint selectedByte)
        {
            //int prevSelected = this.selectedTxt;
            this.selectedByte = selectedByte;

            int elementSize = td.elementSize();
            //Restore table to black
            richTableData.Select(bufTableStart, tableTxtSize);
            richTableData.SelectionColor = Color.Black;
            //New selection:
            selectedTxt = (int)(bufTableStart + 3 * (selectedByte - td.addrInt));
            richTableData.Select(selectedTxt, elementSize * 3 - 1);
            richTableData.SelectionColor = Color.Red;
            richTableData.Select(selectedTxt, 0);
        }

        public void updateTableValues(byte[] tableBuf)
        {
            //After speed optimization it is better to replace whole text
            displayData(selectedByte, tableBuf);
            return;

            try
            {
                this.tableBuf = tableBuf;
                uint tableStart = td.addrInt;
                int elementSize = td.elementSize();

                StringBuilder sb = new StringBuilder();
                int r = 0;
                if (!radioSegmentTBNames.Checked)
                {
                    for (int x=0; x < (numBytesPerRow.Value * 3); x++)
                    {
                        r++;
                        if (x > bufTableStart || richTableData.Text[bufTableStart - x] == '\r')
                            break;
                    }
                }
                r /= 3;
                if (r >= numBytesPerRow.Value)
                {
                    sb.Append(Environment.NewLine);
                    r = 0;
                }
                for (int i=0;i< tableBuf.Length; i++)
                {
                    sb.Append(tableBuf[i].ToString("X2"));
                    r++;
                    if (r >= numBytesPerRow.Value)
                    {
                        sb.Append(Environment.NewLine);
                        r = 0;
                    }
                    else
                    {
                        sb.Append(" ");
                    }

                }
                richTableData.Select(bufTableStart, tableTxtSize);
                richTableData.SelectedText = sb.ToString().Trim();

                richTableData.SelectionColor = Color.Black;
                richTableData.Select(selectedTxt, elementSize * 3 - 1);
                richTableData.SelectionColor = Color.Red;

                richTableData.Select(selectedTxt, 0);
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }


        public void displayData(uint selectedByte, byte[] tableBuf)
        {
            try
            {
                this.selectedByte = selectedByte;
                this.tableBuf = tableBuf;
                uint start = 0;
                uint end = 0;
                uint tableStart = td.startAddress();
                int tableSize = td.size();
                uint tableEnd = td.endAddress();
                int tbHeaderStart = 0;
                int elementSize = td.elementSize();
                int elements = td.elements();
                hexDatas = new List<HexData>();

                int seg = PCM.GetSegmentNumber(td.addrInt);
                if (seg > -1)
                    radioShowSegment.Text = "Segment [" + PCM.Segments[seg].Name +"]";
                if (radioShowTable.Checked)
                {
                    if ((tableStart - numExtraBytes.Value) < 0)
                        start = 0;
                    else
                        start = (uint)(tableStart - numExtraBytes.Value);

                    end = (uint)(tableEnd + numExtraBytes.Value);
                }
                else 
                {
                    start = PCM.segmentinfos[seg].getStartAddr();
                    end = PCM.segmentinfos[seg].getEndAddr();
                }
 
                int bufTableEnd = 0;

                byte[] buf = new byte[end - start + 1];
                Array.Copy(PCM.buf, start, buf, 0, end - start);
                Array.Copy(tableBuf, 0, buf, tableStart - start,tableBuf.Length);
                int r = 0;
                richTableData.Clear();
                if (radioSegmentTBNames.Checked)
                {
                    for (uint a = start; a <= end && a < PCM.fsize; a++)
                    {
                        HexData hd = new HexData();
                        //hd.hex = ToHexString(buf[a - start]);
                        hd.hex = buf[a - start].ToString("X2");
                        hd.prefix = "";
                        hexDatas.Add(hd);
                    }

                    int hexAddr = 0;
                    for (int t = 0; t < PCM.tableDatas.Count; t++)
                    {
                        TableData tmpTd = PCM.tableDatas[t];
                        if (tmpTd.addrInt >= start && tmpTd.addrInt < end)
                        {
                            hexAddr = (int)(tmpTd.startAddress() - start);
                            if (hexAddr > -1 && hexAddr < hexDatas.Count)
                                hexDatas[hexAddr].prefix = tmpTd.TableName + ": " + tmpTd.startAddress().ToString("X8") + " - " + tmpTd.endAddress().ToString("X8") + " [";
                            hexAddr = (int)(tmpTd.endAddress() - start);
                            if (hexAddr > -1 && hexAddr < hexDatas.Count)
                                hexDatas[hexAddr].suffix = "]"; // + PCM.tableDatas[t].TableName; 
                        }
                    }

                    r = 0;
                    StringBuilder sb = new StringBuilder();
                    for (uint h = start; h < end; h++)
                    {
                        int ind = (int)(h - start);
                        if (hexDatas[ind].prefix.Length > 1)
                        {
                            if (r > 0)
                            {
                                if (sb[sb.Length - 1] == ' ')
                                    sb.Remove(sb.Length - 1, 1);
                                sb.Append(Environment.NewLine);
                            }
                            sb.Append(hexDatas[ind].prefix + Environment.NewLine + hexDatas[ind].hex + hexDatas[ind].suffix);
                            r = 0;
                        }
                        else
                        {
                            sb.Append(hexDatas[ind].hex + hexDatas[ind].suffix);
                        }
                        r++;
                        if (r >= numBytesPerRow.Value)
                        {
                            if (sb[sb.Length - 1] == ' ')
                                sb.Remove(sb.Length - 1, 1);
                            sb.Append(Environment.NewLine);
                            r = 0;
                        }
                    }
                    richTableData.Text = sb.ToString();
                    string searchTxt = hexDatas[(int)(tableStart - start)].prefix;
                    tbHeaderStart = richTableData.Find(searchTxt, 0, RichTextBoxFinds.None);
                    bufTableStart = tbHeaderStart + searchTxt.Length + 1;
                    bufTableEnd = richTableData.Find("]", bufTableStart, RichTextBoxFinds.None);
                    tableTxtSize = bufTableEnd - bufTableStart;

                    richTableData.Select(0, tbHeaderStart);
                    richTableData.SelectionColor = Color.Blue;
                    richTableData.Select(bufTableEnd + 1, richTableData.TextLength);
                    richTableData.SelectionColor = Color.Blue;
                    richTableData.Select(tbHeaderStart, tableTxtSize + searchTxt.Length + 1);
                    richTableData.SelectionColor = Color.Black;

                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    for (uint a = start; a <= end && a < PCM.fsize; a++)
                    {
                        sb.Append(buf[a - start].ToString("X2"));
                        if (a == (tableStart - 1))
                            sb.Append("[");
                        else if (a == tableEnd)
                            sb.Append("]");
                        else
                            sb.Append(" ");
                        r++;
                        if (r == numBytesPerRow.Value)
                        {
                            if (sb[sb.Length-1] == ' ')
                                sb.Remove(sb.Length - 1, 1);
                            sb.Append(Environment.NewLine);
                            r = 0;
                        }
                    }
                    richTableData.Text = sb.ToString();
                    bufTableStart = richTableData.Find("[", 0, RichTextBoxFinds.None) + 1;
                    if (richTableData.Text[bufTableStart] == '\n')
                        bufTableStart++;
                    bufTableEnd = richTableData.Find("]", 0, RichTextBoxFinds.None);
                    richTableData.Select(0, bufTableStart - 1);
                    richTableData.SelectionColor = Color.Blue;
                    tableTxtSize = bufTableEnd - bufTableStart;
                    richTableData.Select(bufTableStart, tableTxtSize);
                    richTableData.SelectionColor = Color.Black;
                    richTableData.Select(bufTableEnd +1 , richTableData.TextLength);
                    richTableData.SelectionColor = Color.Blue;

                }

                selectedTxt = (int)(bufTableStart + 3 * (selectedByte - td.addrInt));

                richTableData.Select(selectedTxt, elementSize * 3 - 1);
                richTableData.SelectionColor = Color.Red;

                if (selectedByte - td.addrInt < 100 && radioSegmentTBNames.Checked)
                    richTableData.Select(tbHeaderStart, 0);
                else
                    richTableData.Select(selectedTxt, 0);
            }
            catch(Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }

        private void radioShowSegment_CheckedChanged(object sender, EventArgs e)
        {
            displayData(selectedByte,tableBuf);
        }

        private void radioShowBin_CheckedChanged(object sender, EventArgs e)
        {
            displayData(selectedByte,tableBuf);
        }

        private void numExtraBytes_ValueChanged(object sender, EventArgs e)
        {
            displayData(selectedByte,tableBuf);
        }

        private void numBytesPerRow_ValueChanged(object sender, EventArgs e)
        {
            displayData(selectedByte, tableBuf);
        }

        private void radioSegmentTBNames_CheckedChanged(object sender, EventArgs e)
        {
            displayData(selectedByte, tableBuf);
        }
    }
}
