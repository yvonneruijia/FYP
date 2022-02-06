using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using O2DESNet;


namespace fyp
{
    internal class InboundShipment
    {
        public InboundShipment()
        {
            inboundShipmentLines = new List<InboundShipmentLine>();
            inboundTime = DateTime.MinValue;
        }

        public List<InboundShipmentLine> inboundShipmentLines { get; set; }
        public DateTime inboundTime { get; set; }

        public InboundShipment addShipmentLine(InboundShipmentLine shipment)
        {
            inboundShipmentLines.Add(shipment);
            return this;
        }


        public void listInboundShipments()
        {
            Console.WriteLine("Time of arrival: {0}", inboundTime);

            foreach (var line in inboundShipmentLines)
            {
                Console.WriteLine(line);
            }
        }
    }

    internal class InboundShipmentLine
    {
        public InboundShipmentLine(SKU _sku, int _arrivalQty, int _index) {
            sku = _sku;
            arrivalQty = _arrivalQty;
            index = _index;
        }

        public int arrivalQty { get; set; }
        public int index { get; set; }
        public SKU sku { get; set; }
        public override String ToString()
        {
            return String.Format("SKU {0} ArrivalQty {1} Index {2}: ", sku, arrivalQty, index);
        }
    }

}


