using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace UniversalPatcher
{
    public class UpGauge
    {
        public UpGauge()
        {
            Gauge = new GdiSpeedometerApp.GDISpeedometer();
            Gauge.Width = 200;
            Gauge.Height = 200;
            Gauge.AutoSize = true;
            Gauge.Dock = System.Windows.Forms.DockStyle.Fill;
            TxtBox = new Label();
            TxtBox.AutoSize = true;
            TxtBox.Text = Environment.NewLine + "?".PadLeft(7);
        }
        public enum GaugeType
        {
            Analog,
            Digital,
            Switch
        }
        public string CapText { get { return Gauge.Text; } set { Gauge.Text = value; } }
        public int Row { get; set; }
        public int Column { get; set; }
        public string PidName { get; set; }
        public double MinValue { get { return Gauge.MinSpeed; } set { Gauge.MinSpeed = value; } }
        public double MaxValue { get { return Gauge.MaxSpeed; } set { Gauge.MaxSpeed = value; } }
        public GaugeType Type { get; set; }
        [XmlIgnoreAttribute]
        [Browsable(false)]
        [Bindable(false)]

        public GdiSpeedometerApp.GDISpeedometer Gauge {get;set;}
        [XmlIgnoreAttribute]
        [Browsable(false)]
        [Bindable(false)]
        public Label TxtBox { get; set; }
    }
}
