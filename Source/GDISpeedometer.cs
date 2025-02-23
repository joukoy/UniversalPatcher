using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TestGdiSpeedometerApp
{
    public class GDISpeedometer : Control
    {
        #region private_variables
        private int _size;
        #endregion

        #region private_properties
        private double _minSpeed = 0;
        private double _maxSpeed = 100;
        private int _gaugeThickness = 3;
        private double _speed;
        private Color _gaugeColor = Color.Black;
        private Color _needleColor = Color.Red;
        private bool _showNeedle = true;
        private bool _showGaugeScale = true;
        private Font _currentValueFont = new Font("Arial", 12, FontStyle.Bold);
        #endregion

        public GDISpeedometer()
        {
            DoubleBuffered = true;
            base.Font = new Font("Arial", 8);            
        }

        #region public_properties
        [DefaultValue(0.0)]
        public double Speed
        {
            get { return _speed; }
            set { _speed = value; Invalidate(); }
        }

        public int GaugeThickness
        {
            get { return _gaugeThickness; }
            set { _gaugeThickness = value; Invalidate(); }
        }

        public double MinSpeed
        {
            get { return _minSpeed; }
            set { _minSpeed = value; Invalidate(); }
        }

        public double MaxSpeed
        {
            get { return _maxSpeed; }
            set { _maxSpeed = value; Invalidate(); }
        }

        public Color GaugeColor
        {
            get { return _gaugeColor; }
            set { _gaugeColor = value; Invalidate(); }
        }

        public Color NeedleColor
        {
            get { return _needleColor; }
            set { _needleColor = value; Invalidate(); }
        }

        public bool ShowNeedle
        {
            get { return _showNeedle; }
            set { _showNeedle = value; Invalidate(); }
        }

        public bool ShowGaugeScale
        {
            get { return _showGaugeScale; }
            set { _showGaugeScale = value; Invalidate(); }
        }

        public Font CurrentValueFont
        {
            get { return _currentValueFont; }
            set { _currentValueFont = value; Invalidate(); }
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            //set the minimum width and height (useful at design-time)
            setMinSize();

            //global variables and calculations
            Graphics g = e.Graphics;
            _size = Math.Min(this.ClientSize.Width, this.ClientSize.Height) - _gaugeThickness - 5;
            if (_size <= _gaugeThickness) return;
            float angle = (float)(270 * ((Math.Abs(_minSpeed) + _speed) / (Math.Abs(_minSpeed) + Math.Abs(_maxSpeed))));
            angle = Math.Max(0, Math.Min(270, angle));
            g.SmoothingMode = SmoothingMode.AntiAlias;

            //Whole Arc (all the gauge)
            drawGaugeArc(g);

            //gauge scale
            if (_showGaugeScale)
            {
                drawGaugeScale(g);
            } else
            {
                drawGaugeScaleLimitsOnly(g);
            }

            //gauge needle
            if (_showNeedle)
            {
                drawNeedle(g, angle);
            }

            //display of current value
            drawCurrentValue(g);

            if (base.Text != "")
            {
                drawText(g);
            }

            //Arc (current value)
            drawArcCurrentValue(g, angle);

            base.OnPaint(e);
        }

        private void setMinSize()
        {
            int minWidth = Math.Max(this.ClientSize.Width, 150);
            int minHeight = Math.Max(this.ClientSize.Height, 150);
            this.ClientSize = new Size(minWidth, minHeight);
        }

        private void drawGaugeArc(Graphics g)
        {
            int posX = (this.ClientSize.Width - _size) / 2;
            int posY = ((this.ClientSize.Height - _size) / 2) + _gaugeThickness;
            float centerX = posX + (_size / 2);
            float centerY = posY + (_size / 2);
            using (Pen p = new Pen(_gaugeColor, _gaugeThickness))
            {
                g.DrawArc(p, posX, posY, _size, _size, 135, 270);
            }
        }

        private void drawText(Graphics g)
        {
            int posX = (this.ClientSize.Width - _size) / 2;
            SolidBrush drawBrush = new SolidBrush(base.ForeColor);
            SizeF speedSize = g.MeasureString(base.Text, _currentValueFont);
            float x = posX + (_size / 2) - (speedSize.Width / 2);
            float y = this.ClientSize.Height / 2 - (_size / 6);
            g.DrawString(base.Text, _currentValueFont, drawBrush, x, y);
        }

        private void drawGaugeScale(Graphics g)
        {
            SolidBrush drawBrush = new SolidBrush(base.ForeColor);
            int posX = (this.ClientSize.Width - _size) / 2;
            int posY = ((this.ClientSize.Height - _size) / 2) + _gaugeThickness;
            float centerX = posX + (_size / 2);
            float centerY = posY + (_size / 2);
            int absRange = Convert.ToInt32(Math.Abs(_minSpeed) + Math.Abs(_maxSpeed));
            int step = 0;
            int currVal = 0;
            int boundary = 0;
            if (absRange > 10)
            {
                step = absRange / 10;
                currVal = Convert.ToInt32(_minSpeed);
                boundary = 10;
            }
            else
            {
                step = 1;
                currVal = Convert.ToInt32(_minSpeed);
                boundary = absRange;
            }
            
            for (int i = 0; i <= boundary; i++)
            {
                g.TranslateTransform(centerX, centerY);
                g.RotateTransform(-135 + (27 * i));
                g.TranslateTransform(-centerX, -centerY);
                g.DrawString(Convert.ToString(currVal), base.Font, drawBrush, new Point(Convert.ToInt32(centerX), posY + _gaugeThickness));
                currVal = currVal + step;
                g.ResetTransform();
            }
        }

        private void drawNeedle(Graphics g, float angle)
        {
            int posX = (this.ClientSize.Width - _size) / 2;
            int posY = ((this.ClientSize.Height - _size) / 2) + _gaugeThickness;
            float centerX = posX + (_size / 2);
            float centerY = posY + (_size / 2);
            Pen needlePen = new Pen(_needleColor, 3);            
            g.TranslateTransform(centerX, centerY);
            float angleSingleIncrement = 270 / Convert.ToSingle((Math.Abs(_minSpeed) + Math.Abs(_maxSpeed)));
            g.RotateTransform(-135 + angle); 
            g.TranslateTransform(-centerX, -centerY);
            g.DrawLine(needlePen, new Point(Convert.ToInt32(centerX), posY - _gaugeThickness), new Point(Convert.ToInt32(centerX), Convert.ToInt32(centerY)));
            g.ResetTransform();
        }

        private void drawCurrentValue(Graphics g)
        {
            int posX = (this.ClientSize.Width - _size) / 2;
            SolidBrush drawBrush = new SolidBrush(base.ForeColor);
            String strSpeed;
            if(_speed - Convert.ToInt32(_speed) == 0) 
                strSpeed = _speed.ToString("0");
            else
                strSpeed = _speed.ToString("0.00");
            SizeF speedSize = g.MeasureString(strSpeed, _currentValueFont);
            float x = posX + (_size / 2) - (speedSize.Width / 2);
            float y = this.ClientSize.Height / 2 + (_size / 3);
            g.DrawString(strSpeed, _currentValueFont, drawBrush, x, y);
        }

        private void drawArcCurrentValue(Graphics g, float angle)
        {
            int posX = (this.ClientSize.Width - _size) / 2;
            int posY = ((this.ClientSize.Height - _size) / 2) + _gaugeThickness;
            using (Pen p = new Pen(Color.Green, _gaugeThickness))
            {
                p.Alignment = PenAlignment.Inset;
                g.DrawArc(p, posX, posY, _size, _size, 135, angle);
            }
        }

        private void drawGaugeScaleLimitsOnly(Graphics g)
        {
            SolidBrush drawBrush = new SolidBrush(base.ForeColor);
            int posX = (this.ClientSize.Width - _size) / 2;
            int posY = ((this.ClientSize.Height - _size) / 2) + _gaugeThickness;
            float centerX = posX + (_size / 2);
            float centerY = posY + (_size / 2);
            
            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(-135);
            g.TranslateTransform(-centerX, -centerY);
            g.DrawString(Convert.ToString(_minSpeed), base.Font, drawBrush, new Point(Convert.ToInt32(centerX), posY + _gaugeThickness));
            g.ResetTransform();

            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(135);
            g.TranslateTransform(-centerX, -centerY);
            g.DrawString(Convert.ToString(_maxSpeed), base.Font, drawBrush, new Point(Convert.ToInt32(centerX), posY + _gaugeThickness));
            g.ResetTransform();            
        }

        /**/
    }
}
