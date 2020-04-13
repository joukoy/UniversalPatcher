using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using static upatcher;

namespace UniversalPatcher
{
    public partial class frmCVN : Form
    {
        public frmCVN()
        {
            InitializeComponent();
            RefreshCVNlist();
        }

        private BindingSource CvnSource = new BindingSource();

        public void RefreshCVNlist()
        {
            dataCVN.DataSource = null;
            CvnSource.DataSource = null;
            CvnSource.DataSource = ListCVN;
            dataCVN.DataSource = CvnSource;
        }

        private void btnAddtoStock_Click(object sender, EventArgs e)
        {
            try
            {
                bool isNew = false;
                int counter = 0;
                for (int i = 0; i < ListCVN.Count; i++)
                {
                    CVN stock = ListCVN[i];
                    counter++;
                    if (!CheckStockCVN(stock.PN, stock.Ver, stock.SegmentNr, stock.cvn, false))
                    {
                        //Add if not already in list
                        StockCVN.Add(stock);
                        isNew = true;
                        Debug.WriteLine(stock.PN + " " + stock.Ver + " cvn: " + stock.cvn + " added");
                    }
                    else
                    {
                        Debug.WriteLine(stock.PN + " " + stock.Ver + " cvn: " + stock.cvn + " Already in list");
                    }
                }
                if (counter == 0)
                {
                    Logger("No CVN defined");
                    return;
                }
                if (!isNew)
                {
                    Logger("All segments already in stock list");
                    return;
                }
                Logger("Saving file stockcvn.xml");
                string FileName = Path.Combine(Application.StartupPath, "XML", "stockcvn.xml");
                using (FileStream stream = new FileStream(FileName, FileMode.Create))
                {
                    System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(List<CVN>));
                    writer.Serialize(stream, StockCVN);
                    stream.Close();
                }
                Logger("[OK]");
            }
            catch (Exception ex)
            {
                Logger(ex.Message);
            }

        }

        private void ClearCVNlist()
        {
            ListCVN = new List<CVN>();
            dataCVN.DataSource = null;
            CvnSource.DataSource = null;
            CvnSource.DataSource = ListCVN;
            dataCVN.DataSource = CvnSource;
        }

        private void btnClearCVN_Click(object sender, EventArgs e)
        {
            ClearCVNlist();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshCVNlist();
        }
    }
}
