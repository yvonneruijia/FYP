using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fyp
{
    internal class OutboundOrder
    {
        public OutboundOrder()
        {
            outboundOrderLines = new List<OutboundOrderLine>();
            outboundTime = DateTime.MinValue;
        }

        public List<OutboundOrderLine> outboundOrderLines { get; set;}
        public DateTime outboundTime { get; set; }

        public OutboundOrder addOrderLine(OutboundOrderLine shipment)
        {
            outboundOrderLines.Add(shipment);
            return this;
        }


        public void listOutboundOrders()
        {
            //Console.WriteLine("Time of order: {0}", outboundTime);

            foreach (var line in outboundOrderLines)
            {
                //Console.WriteLine(line);
            }
        }
    }


    internal class OutboundOrderLine
    {
        public OutboundOrderLine(SKU _sku, int _orderQty, int _index)
        {
            sku = _sku;
            orderQty = _orderQty;
            index = _index;
        }
        public int orderQty { get; set; }
        public int index { get; set; }
        public SKU sku { get; set; }
        public override String ToString()
        {
            return String.Format("SKU {0} OrderQty {1} Index {2}: ", sku, orderQty, index);
        }
    }
}