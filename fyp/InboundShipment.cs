using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fyp
{
    internal class InboundShipment
    {
        public InboundShipment() { }
        public List<InboundShipmentLine> inboundShipmentLines { get; set; }
        public float InboundTime { get; set; }
        public void listInboundShipments()
        {
            foreach (var line in inboundShipmentLines)
            {
                Console.WriteLine(line);
            }
        }
    }

    internal class InboundShipmentLine
    {
        public InboundShipmentLine(int _index, SKU _sku, int _arrivalQty) {
            index = _index;
            sku = _sku;
            arrivalQty = _arrivalQty;
        }
        public int arrivalQty { get; set; }
        public int index { get; set; }
        public SKU sku { get; set; }
        //public InboundShipment inboundShipment { get; set; }
        public override String ToString()
        {
            return String.Format("Line{0}: SKU{1} ArrivalQty - {2}", index, sku, arrivalQty);
        }
    }

}


