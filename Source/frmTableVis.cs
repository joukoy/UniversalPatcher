﻿using System;
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
            richTableData.EnableContextMenu();
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

        public void ChangeSelection(uint selectedByte)
        {
            //int prevSelected = this.selectedTxt;
            this.selectedByte = selectedByte;

            int elementSize = td.ElementSize();
            //Restore table to black
            richTableData.Select(bufTableStart, tableTxtSize);
            richTableData.SelectionColor = Color.Black;
            //New selection:
            selectedTxt = (int)(bufTableStart + 3 * (selectedByte - td.StartAddress()));
            richTableData.Select(selectedTxt, elementSize * 3 - 1);
            richTableData.SelectionColor = Color.Red;
            richTableData.Select(selectedTxt, 0);
        }

        public void UpdateTableValues(byte[] tableBuf)
        {
            try
            {
                DisplayData(selectedByte, tableBuf);
                return;
            }
            catch (Exception ex)
            {
                LoggerBold(ex.Message);
            }
        }


        public void DisplayData(uint selectedByte, byte[] tableBuf)
        {
            try
            {
                this.selectedByte = selectedByte;
                this.tableBuf = tableBuf;
                uint start = 0;
                uint end = 0;
                uint tableWithOffsetStart = td.addrInt;
                uint tableStart = td.StartAddress();
                int tableSize = td.Size();
                uint tableEnd = td.EndAddress();
                int tbHeaderStart = 0;
                int elementSize = td.ElementSize();
                int elements = td.Elements();
                hexDatas = new List<HexData>();

                int seg = PCM.GetSegmentNumber(td.addrInt);
                if (seg > -1)
                    radioShowSegment.Text = "Segment [" + PCM.Segments[seg].Name +"]";
                if (radioShowTable.Checked)
                {
                    if ((tableWithOffsetStart - numExtraBytes.Value) < 0)
                        start = 0;
                    else
                        start = (uint)(tableWithOffsetStart - numExtraBytes.Value);

                    end = (uint)(tableEnd + numExtraBytes.Value);
                }
                else 
                {
                    start = PCM.segmentinfos[seg].GetStartAddr();
                    end = PCM.segmentinfos[seg].GetEndAddr();
                }
 
                int bufTableEnd = 0;

                byte[] buf = new byte[end - start + 1];
                Array.Copy(PCM.buf, start, buf, 0, end - start);
                Array.Copy(tableBuf, 0, buf, tableWithOffsetStart - start,tableBuf.Length);
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
                            hexAddr = (int)(tmpTd.addrInt - start);
                            if (hexAddr > -1 && hexAddr < hexDatas.Count)
                            {
                                if (tmpTd.Offset > 0)
                                {
                                    hexDatas[hexAddr].prefix = tmpTd.TableName + ": " + tmpTd.Address + "+" + tmpTd.Offset.ToString("X") + " - " + tmpTd.EndAddress().ToString("X8");
                                    hexDatas[hexAddr + tmpTd.Offset - 1].suffix = "[";
                                }
                                else
                                    hexDatas[hexAddr].prefix = tmpTd.TableName + ": " + tmpTd.Address + " - " + tmpTd.EndAddress().ToString("X8") + " [";
                            }
                            hexAddr = (int)(tmpTd.EndAddress() - start);
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
                    string searchTxt = hexDatas[(int)(tableWithOffsetStart - start)].prefix;
                    tbHeaderStart = richTableData.Find(searchTxt, 0, RichTextBoxFinds.None);
                    bufTableStart = tbHeaderStart + searchTxt.Length + 1 + (td.Offset * 3);
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
                if (td.Offset > 0)
                {
                    richTableData.Select(bufTableStart - (3 * td.Offset), 3 * td.Offset - 1);
                    richTableData.SelectionColor = Color.Green;
                }


                selectedTxt = (int)(bufTableStart + 3 * (selectedByte - tableStart));

                richTableData.Select(selectedTxt, elementSize * 3 - 1);
                richTableData.SelectionColor = Color.Red;

                if (selectedByte - td.StartAddress() < 100 && radioSegmentTBNames.Checked)
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
            DisplayData(selectedByte,tableBuf);
        }

        private void radioShowBin_CheckedChanged(object sender, EventArgs e)
        {
            DisplayData(selectedByte,tableBuf);
        }

        private void numExtraBytes_ValueChanged(object sender, EventArgs e)
        {
            DisplayData(selectedByte,tableBuf);
        }

        private void numBytesPerRow_ValueChanged(object sender, EventArgs e)
        {
            DisplayData(selectedByte, tableBuf);
        }

        private void radioSegmentTBNames_CheckedChanged(object sender, EventArgs e)
        {
            DisplayData(selectedByte, tableBuf);
        }
    }
}
