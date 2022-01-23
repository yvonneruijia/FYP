using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fyp
{
    internal class SKU
    {
        public SKU (int _index)
        {
            index = _index;
        }

        public int index { get; set; }
        public OutboundOrderLine OutboundOrderLine { get; set; }
        public StorageLine StorageLine { get; set; }
        public InboundShipmentLine InboundShipmentLine { get; set; }
        public Popularity Popularity { get; set; }
        public int MaxStorageQty { get; set; }
        public int TotalQty { get; set; } /// Q_System = sum of Q(i,j) whr jinJ
        public float X_avg { get; set; } /// use formula, xi, Q(i,j) info
        public float Y_avg { get; set; } /// use similar formula
        public float Z_avg { get; set; }
        public float New_X_avg { get; set; } /// use formula, xi, Q(i,j) info
        public float New_Y_avg { get; set; } /// use similar formula
        public float New_Z_avg { get; set; }

        public override String ToString()
        {
            return String.Format("{0}", index);
        }


    }
}
