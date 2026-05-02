using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UniversalPatcher
{
    public class ReDo
    {
        public ReDo() 
        {
            Time = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
        }
        public ReDo(Object Obj, Object ObjCollection,string Collection, string ItemName, string Property, RedoAction Action, Object OldValue, Object NewValue, int Position)
        {
            Time = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
            this.Obj = Obj;
            this.Collection = Collection;
            this.ItemName = ItemName;
            this.ObjCollection = ObjCollection;
            this.Property = Property;
            this.Action = Action;
            this.OldValue = OldValue;
            this.NewValue = NewValue;
            this.Position = Position;
        }

        public enum RedoAction
        {
            Add,
            Edit,
            Delete
        }
        public string Time { get; set; }
        public string Collection { get; set; }
        public string ItemName { get; set; }
        public string Property { get; set; }
        public RedoAction Action { get; set; }        
        public Object OldValue { get; set; }
        public Object NewValue { get; set; }
        public int Position { get; set; }
        [Browsable(false)]
        [Bindable(false)]
        public Object Obj { get; set; }
        [Browsable(false)]
        [Bindable(false)]
        public Object ObjCollection { get; set; }
        public string CsvHeader()
        {
            return "Time;Collection;ItemName;Property;Action;OldValue;NewValue;Position;Object";
        }
        public string CsvLine()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Time + ";");
            sb.Append(Collection + ";");
            sb.Append(ItemName + ";");
            sb.Append(Property + ";");
            sb.Append(Action.ToString() + ";");
            sb.Append(OldValue.ToString() + ";");
            sb.Append(NewValue.ToString() + ";");
            sb.Append(Position.ToString() + ";");
            sb.Append(Obj.ToString() + ";");
            return sb.ToString();
        }
    }
}
