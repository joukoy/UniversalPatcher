using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using static upatcher;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace UniversalPatcher
{
    class TinyTuner
    {
        private string RemoveDuplicates(string colHeaders)
        {
            string retVal = "";
            string[] parts = colHeaders.Split(',');
            if (parts.Length < 2) return colHeaders;
            for (int a= 0; a< parts.Length; a++)
            {
                int d = 0;
                for (int b= a+1; b < parts.Length; b++)
                {
                    if (parts[a] == parts[b])
                    {
                        d++;
                        parts[b] += d.ToString();
                    }
                }
                retVal += parts[a];
                if (a < parts.Length - 1) retVal += ",";

            }

            return retVal;
        }

        public string convertByHeader(string header, int count)
        {
            string retVal = "";
            string[] parts = header.Split(',');
            if (parts.Length == 3 )
            {
                int from = Convert.ToInt32(parts[0]);
                int step = Convert.ToInt32(parts[2]);
                for (int i = 0; i < count; i++)
                {
                    retVal += (from + i * step).ToString();
                    if (i < count - 1) retVal += ",";
                }
                return retVal;
            }
            return header;
        }
        public string readTinyDB(PcmFile PCM)
        {
            string connetionString = null;
            OleDbConnection cnn;
            string dbFile = Path.Combine(Application.StartupPath, "TinyTuner.mdb");
            connetionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbFile;
            cnn = new OleDbConnection(connetionString);
            try
            {
                string query = "Select MapNumber from OSIDData where OSID=" + PCM.OS.ToString();
                OleDbCommand selectCommand = new OleDbCommand(query, cnn);
                cnn.Open();
                DataTable table = new DataTable();
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                adapter.SelectCommand = selectCommand;
                adapter.Fill(table);
                string mapNr = "";
                foreach (DataRow row in table.Rows)
                {
                    mapNr = row["MapNumber"].ToString();
                }
                if (mapNr == "")
                {
                    MessageBox.Show("OS not found from TinyTuner DB", "OS not found from TinyTuner DB");
                    return "Not found";
                }

                query = "select * from CategoryList order by Category";
                selectCommand = new OleDbCommand(query, cnn);
                table = new DataTable();
                adapter = new OleDbDataAdapter();
                adapter.SelectCommand = selectCommand;
                adapter.Fill(table);

                if (PCM.tableCategories == null) PCM.tableCategories = new List<string>();
                foreach (DataRow row in table.Rows)
                {
                    string cat = row["Category"].ToString();
                    if (!PCM.tableCategories.Contains(cat))
                    {
                        PCM.tableCategories.Add(cat);
                    }
                }

                query = "select * from TableData where MapNumber=" + mapNr + " order by TableName";
                selectCommand = new OleDbCommand(query, cnn);
                table = new DataTable();
                adapter = new OleDbDataAdapter();
                adapter.SelectCommand = selectCommand;
                adapter.Fill(table);

                if (tableSeeks == null) tableSeeks = new List<TableSeek>();
                if (PCM.foundTables == null) PCM.foundTables = new List<FoundTable>();
                foreach (DataRow row in table.Rows)
                {
                    TableSeek ts = new TableSeek();
                    FoundTable ft = new FoundTable(PCM);

                    HexToUint(row["StartPosition"].ToString(), out ft.addrInt);
                    ft.Address = ft.addrInt.ToString("X8");
                    ft.Columns = Convert.ToUInt16(row["ColumnCount"]);
                    ft.Description = row["TableDescription"].ToString();
                    ft.Name = row["TableName"].ToString();
                    ft.Rows = Convert.ToUInt16(row["RowCount"]);
                    ft.Category = row["MainCategory"].ToString();
                    ft.configId = tableSeeks.Count;
                    PCM.foundTables.Add(ft);

                    ushort elementSize = (ushort) (Convert.ToUInt16(row["ElementSize"]));
                    bool signed = Convert.ToBoolean(row["AllowNegative"]);
                    ts.DataType = convertToDataType(elementSize, signed, true);
                    string colHeaders = row["ColumnHeaders"].ToString();
                    ts.ColHeaders = RemoveDuplicates(colHeaders);
                    ts.Columns = Convert.ToUInt16(row["ColumnCount"]);
                    ts.Decimals = 2;
                    ts.Description = row["TableDescription"].ToString();
                    ts.Math = "X*" + row["Factor"].ToString();
                    ts.Name = row["TableName"].ToString();
                    ts.Rows = Convert.ToUInt16(row["RowCount"]);
                    ts.RowHeaders = row["RowHeaders"].ToString();
                    if (ts.RowHeaders.Contains(",by,"))
                    {
                        ts.RowHeaders = convertByHeader(ts.RowHeaders,ts.Rows);
                    }
                    ts.SavingMath = "X/" + row["Factor"].ToString();
                    ts.Category = row["MainCategory"].ToString();
                    ts.Units = row["Units"].ToString();
                    ts.RowMajor = false;
                    tableSeeks.Add(ts);                    
                }


                cnn.Close();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                MessageBox.Show("Error, TinyTuner line " + line + ": " + ex.Message, "Error");
            }
            return "OK";
        }
        public string readTinyDBtoTableData(PcmFile PCM, List<TableData> tdList)
        {
            string connetionString = null;
            OleDbConnection cnn;
            string dbFile = Path.Combine(Application.StartupPath, "TinyTuner.mdb");
            connetionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbFile;
            cnn = new OleDbConnection(connetionString);
            try
            {
                string query = "Select MapNumber from OSIDData where OSID=" + PCM.OS;
                OleDbCommand selectCommand = new OleDbCommand(query, cnn);
                cnn.Open();
                DataTable table = new DataTable();
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                adapter.SelectCommand = selectCommand;
                adapter.Fill(table);
                string mapNr = "";
                foreach (DataRow row in table.Rows)
                {
                    mapNr = row["MapNumber"].ToString();
                }
                if (mapNr == "")
                {
                    LoggerBold("OS not found from TinyTuner DB");
                    return "Not found";
                }

                query = "select * from CategoryList order by Category";
                selectCommand = new OleDbCommand(query, cnn);
                table = new DataTable();
                adapter = new OleDbDataAdapter();
                adapter.SelectCommand = selectCommand;
                adapter.Fill(table);

                if (PCM.tableCategories == null) PCM.tableCategories = new List<string>();
                foreach (DataRow row in table.Rows)
                {
                    string cat = row["Category"].ToString();
                    if (!PCM.tableCategories.Contains(cat))
                    {
                        PCM.tableCategories.Add(cat);
                    }
                }

                query = "select * from TableData where MapNumber=" + mapNr + " order by TableName";
                selectCommand = new OleDbCommand(query, cnn);
                table = new DataTable();
                adapter = new OleDbDataAdapter();
                adapter.SelectCommand = selectCommand;
                adapter.Fill(table);

                if (tableSeeks == null) tableSeeks = new List<TableSeek>();
                if (PCM.foundTables == null) PCM.foundTables = new List<FoundTable>();
                foreach (DataRow row in table.Rows)
                {
                    TableData td = new TableData();

                    td.OS = PCM.OS;
                    td.Origin = "TinyTuner";
                    HexToUint(row["StartPosition"].ToString(), out td.addrInt);
                    //td.Address = td.AddrInt.ToString("X8");
                    td.Columns = Convert.ToUInt16(row["ColumnCount"]);
                    td.TableDescription = row["TableDescription"].ToString();
                    td.TableName = row["TableName"].ToString().Replace("."," ");
                    td.Rows = Convert.ToUInt16(row["RowCount"]);
                    td.Category = row["MainCategory"].ToString();

                    int elementSize = (byte)(Convert.ToByte(row["ElementSize"]));
                    bool Signed = Convert.ToBoolean(row["AllowNegative"]);
                    td.DataType = convertToDataType(elementSize, Signed, false);
                    string colHeaders = row["ColumnHeaders"].ToString();
                    td.ColumnHeaders = RemoveDuplicates(colHeaders);
                    //td.Floating = true;
                    td.Decimals = 2;
                    if (row["Factor"].ToString().Length > 0)
                        td.Math = "X*" + row["Factor"].ToString();
                    else
                        td.Math = "X*1";
                    td.RowHeaders = row["RowHeaders"].ToString();
                    if (td.RowHeaders.Contains(",by,"))
                    {
                        td.RowHeaders = convertByHeader(td.RowHeaders, td.Rows);
                    }
                    //if (row["Factor"].ToString().Length > 0)
                        //td.SavingMath = "X/" + row["Factor"].ToString();
                    //else
                        //td.SavingMath = "X/1";
                    td.Category = row["MainCategory"].ToString();
                    if (!PCM.tableCategories.Contains(td.Category))
                        PCM.tableCategories.Add(td.Category);
                    td.Units = row["Units"].ToString();
                    td.RowMajor = false;
                    tdList.Add(td);
                }


                cnn.Close();
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, TinyTuner line " + line + ": " + ex.Message);
            }
            return "OK";
        }

    }
}
