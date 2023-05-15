using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static Upatcher;
using static Helpers;

namespace UniversalPatcher
{
    public class V6CrossRef
    {
        public OleDbConnection v6refConn;
        public OleDbDataAdapter stockAdapter;
        public OleDbDataAdapter refAdapter;
        public DataTable v6RefTb;

        public void LoadDB()
        {
            try
            {
                string dbfile = Path.Combine(Application.StartupPath, "db", "V6CrossRef.mdb");
                string connetionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbfile;
                v6refConn = new OleDbConnection(connetionString);
                string query = "Select * from OSid";
                OleDbCommand selectCommand = new OleDbCommand(query, v6refConn);
                v6refConn.Open();
                v6RefTb = new DataTable();
                stockAdapter = new OleDbDataAdapter();
                stockAdapter.SelectCommand = selectCommand;
                stockAdapter.Fill(v6RefTb);

            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, V6CrossRef line " + line + ": " + ex.Message);
            }
        }

        public void AddtoRef(uint OS, string CS1Addr,string OsStroeAddr, string MafAddr, string VeTable, string CalStart, string Crc, string Tables3d)
        {
            try
            {
                OleDbCommandBuilder bldr = new OleDbCommandBuilder(stockAdapter);

                DataRow row = v6RefTb.NewRow();

                row["OS"] = OS;
                row["CRC"] = Crc;
                row["CS1Address"] = CS1Addr;
                row["3dTables"] = Tables3d;
                row["MAFAddress"] = MafAddr;
                row["VETable"] = VeTable;
                row["CalStart"] = CalStart;
                row["OSStoreAddr"] = OsStroeAddr;


                v6RefTb.Rows.Add(row);
                stockAdapter.InsertCommand = bldr.GetInsertCommand();
                int updatedRows = stockAdapter.Update(v6RefTb);
                if (updatedRows > 0)
                    Debug.WriteLine("Updated :" + updatedRows.ToString() + " rows in v6RefTb db");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, V6CrossRef line " + line + ": " + ex.Message);

            }

        }

        public uint[] GetCompatibleOSs(string Crc)
        {
            uint[] retval = null;
            try
            {
                string qry = "CRC = '" + Crc + "'";
                DataRow[] res = v6RefTb.Select(qry);
                if (res.Length > 0)
                {
                    retval = new uint[res.Length];
                    for (int i = 0; i < res.Length; i++)
                    {
                        retval[i] = Convert.ToUInt32(res[i]["OS"]);
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
                LoggerBold("Error, V6CrossRef line " + line + ": " + ex.Message);

            }
            return retval;
        }

        public uint[] GetLessCompatibleOSs(PcmFile PCM)
        {
            uint[] retval = null;
            try
            {
                string v6tablelist = "";
                for (int i = 0; i < PCM.v6tables.Count; i++)
                {
                    if (i > 0)
                        v6tablelist += ",";
                    v6tablelist += PCM.v6tables[i].address.ToString("X") + ":" + PCM.v6tables[i].rows.ToString();
                }

                string qry = "CS1Address = '" + PCM.segmentAddressDatas[0].CS1Address.Address.ToString("X") + "' AND 3DTables = '" + v6tablelist + "'";
                DataRow[] res = v6RefTb.Select(qry);
                if (res.Length > 0)
                {
                    retval = new uint[res.Length];
                    for (int i = 0; i < res.Length; i++)
                    {
                        retval[i] = Convert.ToUInt32(res[i]["OS"]);
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
                LoggerBold("Error, V6CrossRef line " + line + ": " + ex.Message);

            }
            return retval;
        }

    }
}
