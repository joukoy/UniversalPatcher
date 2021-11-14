using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static upatcher;

namespace UniversalPatcher
{
    public class CvnDB
    {
        public OleDbConnection cvnConn;
        public OleDbDataAdapter stockAdapter;
        public OleDbDataAdapter refAdapter;
        public DataTable stockCvn;
        public DataTable refCvn;

        public void loadDB()
        {
            try
            {
                string dbfile = Path.Combine(Application.StartupPath, "db", "cvn.mdb");
                string connetionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dbfile;
                cvnConn = new OleDbConnection(connetionString);
                string query = "Select * from cvn";
                OleDbCommand selectCommand = new OleDbCommand(query, cvnConn);
                cvnConn.Open();
                stockCvn = new DataTable();
                stockAdapter = new OleDbDataAdapter();
                stockAdapter.SelectCommand = selectCommand;
                stockAdapter.Fill(stockCvn);

                query = "Select * from referencecvn";
                selectCommand = new OleDbCommand(query, cvnConn);
                refCvn = new DataTable();
                refAdapter = new OleDbDataAdapter();
                refAdapter.SelectCommand = selectCommand;
                refAdapter.Fill(refCvn);
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, cvnDB line " + line + ": " + ex.Message);

            }
        }

        public void addtoStock(CVN cvn)
        {
            try
            {
                OleDbCommandBuilder bldr = new OleDbCommandBuilder(stockAdapter);

                DataRow row = stockCvn.NewRow();

                row["pn"] = cvn.PN;
                row["cvn"] = cvn.cvn;
                row["segmentnr"] = cvn.SegmentNr;
                row["ver"] = cvn.Ver;
                row["xmlfile"] = cvn.XmlFile;

                string qry = "pn = '" + cvn.PN + "'";
                DataRow[] res = refCvn.Select(qry);

                if (res.Length > 0)
                    row["referencecvn"] = res[0]["cvn"].ToString().TrimStart('0');
                stockCvn.Rows.Add(row);
                stockAdapter.InsertCommand = bldr.GetInsertCommand();
                int updatedRows = stockAdapter.Update(stockCvn);
                if (updatedRows > 0)
                    Debug.WriteLine("Updated :" + updatedRows.ToString() + " rows in stockcvn db");
            }
            catch (Exception ex)
            {
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                LoggerBold("Error, cvnDB line " + line + ": " + ex.Message);

            }

        }

        public string getStockCvn(string PN, string Ver, string SegNr)
        {
            string retval = "";
            try
            {
                string qry = "pn = '" + PN + "' AND ver = '" + Ver + "' AND segmentnr = " + SegNr.ToString();
                DataRow[] res = stockCvn.Select(qry);
                if (res.Length > 0)
                {
                    retval = res[0]["cvn"].ToString();
                }
                else
                {
                    qry = "pn = '" + PN + "'";
                    res = refCvn.Select(qry);
                    if (res.Length > 0)
                    {
                        retval = res[0]["cvn"].ToString();
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
                LoggerBold("Error, cvnDB line " + line + ": " + ex.Message);

            }
            return retval;
        }
    }
}
