using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    public class PopupWindow : System.Windows.Forms.ToolStripDropDown
    {
        private System.Windows.Forms.Control _content;
        private System.Windows.Forms.ToolStripControlHost _host;

        public PopupWindow(System.Windows.Forms.Control content, Point position)
        {
            //Basic setup...
            this.AutoSize = false;
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;

            this._content = content;
            this._host = new System.Windows.Forms.ToolStripControlHost(content);

            //Positioning and Sizing
            this.MinimumSize = content.MinimumSize;
            this.MaximumSize = content.Size;
            this.Size = content.Size;
            content.Location = Point.Empty;

            //Add the host to the list
            this.Items.Add(this._host);
        }
    }
}
